-- ============================================================================
-- Auto-create user_profiles row when a new user signs up
-- ============================================================================
-- This trigger fires AFTER a new row is inserted into auth.users.
-- It creates the corresponding user_profiles row automatically,
-- so the client-side code does NOT need an access_token to insert
-- the profile (which fails when email confirmation is enabled).
-- ============================================================================

CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER          -- runs with owner privileges, bypasses RLS
SET search_path = public  -- prevent search_path hijacking
AS $$
BEGIN
  INSERT INTO public.user_profiles (id, full_name, role)
  VALUES (
    NEW.id,
    COALESCE(NEW.raw_user_meta_data ->> 'full_name', ''),
    'customer'
  )
  ON CONFLICT (id) DO NOTHING;   -- idempotent: skip if profile already exists

  RETURN NEW;
END;
$$;

-- Fire after every INSERT on auth.users
CREATE TRIGGER on_auth_user_created
  AFTER INSERT ON auth.users
  FOR EACH ROW
  EXECUTE FUNCTION public.handle_new_user();

-- Also backfill any existing auth.users that are missing a profile
INSERT INTO public.user_profiles (id, full_name, role)
SELECT
  u.id,
  COALESCE(u.raw_user_meta_data ->> 'full_name', ''),
  'customer'
FROM auth.users u
LEFT JOIN public.user_profiles p ON p.id = u.id
WHERE p.id IS NULL
ON CONFLICT (id) DO NOTHING;
