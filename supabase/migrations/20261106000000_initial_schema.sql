-- ============================================================================
-- LokyPet E-Commerce — Initial Schema Migration
-- Generated: 2026-11-06
-- ============================================================================
-- This migration creates the full database schema including:
--   1.  categories
--   2.  products
--   3.  product_variants
--   4.  product_images
--   5.  user_profiles
--   6.  orders
--   7.  order_items
--   8.  inventory_movements
--   9.  contact_messages
--   +   Helper functions, triggers, RLS policies, storage bucket
-- ============================================================================


-- ============================================================================
-- 0. EXTENSIONS
-- ============================================================================
CREATE EXTENSION IF NOT EXISTS "pgcrypto";   -- provides gen_random_uuid()


-- ============================================================================
-- 1. CATEGORIES
-- ============================================================================
CREATE TABLE categories (
  id         uuid        PRIMARY KEY DEFAULT gen_random_uuid(),
  name       text        NOT NULL,
  slug       text        NOT NULL UNIQUE,
  sort_order int         DEFAULT 0,
  created_at timestamptz DEFAULT now(),
  updated_at timestamptz DEFAULT now()
);

COMMENT ON TABLE categories IS 'Product categories for the catalog';


-- ============================================================================
-- 2. PRODUCTS
-- ============================================================================
CREATE TABLE products (
  id          uuid        PRIMARY KEY DEFAULT gen_random_uuid(),
  category_id uuid        NOT NULL REFERENCES categories(id),
  name        text        NOT NULL,
  slug        text        NOT NULL UNIQUE,
  description text        DEFAULT '',
  is_active   boolean     DEFAULT true,
  created_at  timestamptz DEFAULT now(),
  updated_at  timestamptz DEFAULT now()
);

CREATE INDEX idx_products_category_id ON products (category_id);
CREATE INDEX idx_products_is_active   ON products (is_active);

COMMENT ON TABLE products IS 'Base products; each product has one or more variants';


-- ============================================================================
-- 3. PRODUCT VARIANTS
-- ============================================================================
CREATE TABLE product_variants (
  id                    uuid          PRIMARY KEY DEFAULT gen_random_uuid(),
  product_id            uuid          NOT NULL REFERENCES products(id),
  sku                   text          NOT NULL UNIQUE,
  slug                  text          NOT NULL UNIQUE,
  color_name            text          NOT NULL,
  color_hex             text[]        NOT NULL DEFAULT '{}',
  price_mxn             numeric(10,2) NOT NULL,
  compare_at_price_mxn  numeric(10,2) NULL,
  stock                 int           NOT NULL DEFAULT 0 CHECK (stock >= 0),
  is_out_of_stock       boolean       DEFAULT false,
  is_active             boolean       DEFAULT true,
  created_at            timestamptz   DEFAULT now(),
  updated_at            timestamptz   DEFAULT now()
);

CREATE INDEX idx_product_variants_product_id ON product_variants (product_id);
CREATE INDEX idx_product_variants_is_active  ON product_variants (is_active);

COMMENT ON TABLE product_variants IS 'Purchasable variants of a product (color, price, stock)';
COMMENT ON COLUMN product_variants.price_mxn            IS 'Final / sale price in MXN';
COMMENT ON COLUMN product_variants.compare_at_price_mxn IS 'Original "before" price; if > price_mxn the item is on sale';


-- ============================================================================
-- 4. PRODUCT IMAGES
-- ============================================================================
CREATE TABLE product_images (
  id           uuid        PRIMARY KEY DEFAULT gen_random_uuid(),
  product_id   uuid        NOT NULL REFERENCES products(id),
  variant_id   uuid        NULL REFERENCES product_variants(id),
  storage_path text        NOT NULL,
  sort_order   int         DEFAULT 0,
  alt_text     text        DEFAULT '',
  created_at   timestamptz DEFAULT now()
);

CREATE INDEX idx_product_images_product_id ON product_images (product_id);
CREATE INDEX idx_product_images_variant_id ON product_images (variant_id);

COMMENT ON TABLE product_images IS 'Images stored in Supabase Storage, linked to products/variants';


-- ============================================================================
-- 5. USER PROFILES
-- ============================================================================
CREATE TABLE user_profiles (
  id              uuid        PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
  full_name       text        DEFAULT '',
  phone           text        NULL,
  default_address jsonb       NULL,
  avatar_key      text        NULL,
  role            text        NOT NULL DEFAULT 'customer'
                              CHECK (role IN ('customer', 'admin')),
  created_at      timestamptz DEFAULT now(),
  updated_at      timestamptz DEFAULT now()
);

COMMENT ON TABLE user_profiles IS 'Extended profile for each auth.users row';


-- ============================================================================
-- 6. ORDERS
-- ============================================================================
CREATE TABLE orders (
  id               uuid          PRIMARY KEY DEFAULT gen_random_uuid(),
  order_number     text          NOT NULL UNIQUE,
  user_id          uuid          NULL REFERENCES auth.users(id),
  customer_name    text          NOT NULL,
  customer_email   text          NOT NULL,
  customer_phone   text          DEFAULT '',
  shipping_address text          NOT NULL,
  shipping_city    text          NOT NULL,
  shipping_state   text          NOT NULL,
  shipping_zip_code text         DEFAULT '',
  shipping_type    text          NOT NULL CHECK (shipping_type IN ('local', 'nacional')),
  shipping_carrier text          NULL,
  tracking_number  text          NULL,
  subtotal         numeric(10,2) NOT NULL,
  discount_total   numeric(10,2) DEFAULT 0,
  shipping_cost    numeric(10,2) NOT NULL DEFAULT 0,
  total            numeric(10,2) NOT NULL,
  status           text          NOT NULL DEFAULT 'paid'
                                 CHECK (status IN ('paid', 'preparing', 'shipped', 'delivered', 'cancelled')),
  is_simulated     boolean       DEFAULT true,
  created_at       timestamptz   DEFAULT now(),
  updated_at       timestamptz   DEFAULT now()
);

CREATE INDEX idx_orders_user_id ON orders (user_id);
CREATE INDEX idx_orders_status  ON orders (status);

COMMENT ON TABLE orders IS 'Customer orders';


-- ============================================================================
-- 7. ORDER ITEMS
-- ============================================================================
CREATE TABLE order_items (
  id               uuid          PRIMARY KEY DEFAULT gen_random_uuid(),
  order_id         uuid          NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
  variant_id       uuid          NOT NULL REFERENCES product_variants(id),
  sku              text          NOT NULL,
  name             text          NOT NULL,
  color            text          DEFAULT '',
  unit_price       numeric(10,2) NOT NULL,
  compare_at_price numeric(10,2) NULL,
  quantity         int           NOT NULL CHECK (quantity > 0),
  line_total       numeric(10,2) NOT NULL
);

CREATE INDEX idx_order_items_order_id ON order_items (order_id);

COMMENT ON TABLE order_items IS 'Snapshot of each line item within an order';


-- ============================================================================
-- 8. INVENTORY MOVEMENTS
-- ============================================================================
CREATE TABLE inventory_movements (
  id         uuid        PRIMARY KEY DEFAULT gen_random_uuid(),
  variant_id uuid        NOT NULL REFERENCES product_variants(id),
  delta      int         NOT NULL,
  reason     text        DEFAULT '',
  user_id    uuid        NULL REFERENCES auth.users(id),
  created_at timestamptz DEFAULT now()
);

CREATE INDEX idx_inventory_movements_variant_id ON inventory_movements (variant_id);

COMMENT ON TABLE inventory_movements IS 'Audit log for stock changes (+/-)';


-- ============================================================================
-- 9. CONTACT MESSAGES
-- ============================================================================
CREATE TABLE contact_messages (
  id         uuid        PRIMARY KEY DEFAULT gen_random_uuid(),
  name       text        NOT NULL,
  email      text        NOT NULL,
  phone      text        NULL,
  subject    text        NOT NULL,
  message    text        NOT NULL,
  status     text        DEFAULT 'nuevo'
                         CHECK (status IN ('nuevo', 'leido', 'respondido')),
  created_at timestamptz DEFAULT now(),
  updated_at timestamptz DEFAULT now()
);

COMMENT ON TABLE contact_messages IS 'Messages submitted via the public contact form';


-- ============================================================================
-- HELPER FUNCTION: updated_at TRIGGER
-- ============================================================================
CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
  NEW.updated_at = now();
  RETURN NEW;
END;
$$;

COMMENT ON FUNCTION set_updated_at() IS 'Automatically sets updated_at to now() on every UPDATE';

-- Apply the trigger to every table that has an updated_at column
CREATE TRIGGER trg_categories_updated_at
  BEFORE UPDATE ON categories
  FOR EACH ROW EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_products_updated_at
  BEFORE UPDATE ON products
  FOR EACH ROW EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_product_variants_updated_at
  BEFORE UPDATE ON product_variants
  FOR EACH ROW EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_user_profiles_updated_at
  BEFORE UPDATE ON user_profiles
  FOR EACH ROW EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_orders_updated_at
  BEFORE UPDATE ON orders
  FOR EACH ROW EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_contact_messages_updated_at
  BEFORE UPDATE ON contact_messages
  FOR EACH ROW EXECUTE FUNCTION set_updated_at();


-- ============================================================================
-- HELPER FUNCTION: is_admin()
-- ============================================================================
CREATE OR REPLACE FUNCTION is_admin()
RETURNS boolean
LANGUAGE sql STABLE SECURITY DEFINER
AS $$
  SELECT EXISTS (
    SELECT 1 FROM user_profiles
    WHERE id = auth.uid() AND role = 'admin'
  );
$$;

COMMENT ON FUNCTION is_admin() IS 'Returns true when the current authenticated user has the admin role';


-- ============================================================================
-- ROW LEVEL SECURITY — ENABLE ON ALL TABLES
-- ============================================================================
ALTER TABLE categories          ENABLE ROW LEVEL SECURITY;
ALTER TABLE products            ENABLE ROW LEVEL SECURITY;
ALTER TABLE product_variants    ENABLE ROW LEVEL SECURITY;
ALTER TABLE product_images      ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_profiles       ENABLE ROW LEVEL SECURITY;
ALTER TABLE orders              ENABLE ROW LEVEL SECURITY;
ALTER TABLE order_items         ENABLE ROW LEVEL SECURITY;
ALTER TABLE inventory_movements ENABLE ROW LEVEL SECURITY;
ALTER TABLE contact_messages    ENABLE ROW LEVEL SECURITY;


-- ============================================================================
-- RLS POLICIES — CATEGORIES (public catalog)
-- ============================================================================
CREATE POLICY "categories: public read"
  ON categories FOR SELECT
  USING (true);

CREATE POLICY "categories: admin insert"
  ON categories FOR INSERT
  WITH CHECK (is_admin());

CREATE POLICY "categories: admin update"
  ON categories FOR UPDATE
  USING (is_admin());

CREATE POLICY "categories: admin delete"
  ON categories FOR DELETE
  USING (is_admin());


-- ============================================================================
-- RLS POLICIES — PRODUCTS (public catalog, active only)
-- ============================================================================
CREATE POLICY "products: public read active"
  ON products FOR SELECT
  USING (is_active = true OR is_admin());

CREATE POLICY "products: admin insert"
  ON products FOR INSERT
  WITH CHECK (is_admin());

CREATE POLICY "products: admin update"
  ON products FOR UPDATE
  USING (is_admin());

CREATE POLICY "products: admin delete"
  ON products FOR DELETE
  USING (is_admin());


-- ============================================================================
-- RLS POLICIES — PRODUCT VARIANTS (public catalog, active only)
-- ============================================================================
CREATE POLICY "product_variants: public read active"
  ON product_variants FOR SELECT
  USING (is_active = true OR is_admin());

CREATE POLICY "product_variants: admin insert"
  ON product_variants FOR INSERT
  WITH CHECK (is_admin());

CREATE POLICY "product_variants: admin update"
  ON product_variants FOR UPDATE
  USING (is_admin());

CREATE POLICY "product_variants: admin delete"
  ON product_variants FOR DELETE
  USING (is_admin());


-- ============================================================================
-- RLS POLICIES — PRODUCT IMAGES (public read)
-- ============================================================================
CREATE POLICY "product_images: public read"
  ON product_images FOR SELECT
  USING (true);

CREATE POLICY "product_images: admin insert"
  ON product_images FOR INSERT
  WITH CHECK (is_admin());

CREATE POLICY "product_images: admin update"
  ON product_images FOR UPDATE
  USING (is_admin());

CREATE POLICY "product_images: admin delete"
  ON product_images FOR DELETE
  USING (is_admin());


-- ============================================================================
-- RLS POLICIES — USER PROFILES
-- ============================================================================
CREATE POLICY "user_profiles: read own"
  ON user_profiles FOR SELECT
  USING (auth.uid() = id OR is_admin());

CREATE POLICY "user_profiles: insert own"
  ON user_profiles FOR INSERT
  WITH CHECK (auth.uid() = id);

CREATE POLICY "user_profiles: update own"
  ON user_profiles FOR UPDATE
  USING (auth.uid() = id);


-- ============================================================================
-- RLS POLICIES — ORDERS
-- ============================================================================
CREATE POLICY "orders: read own or admin"
  ON orders FOR SELECT
  USING (user_id = auth.uid() OR is_admin());

CREATE POLICY "orders: insert authenticated"
  ON orders FOR INSERT
  WITH CHECK (auth.uid() IS NOT NULL);

CREATE POLICY "orders: admin update"
  ON orders FOR UPDATE
  USING (is_admin());


-- ============================================================================
-- RLS POLICIES — ORDER ITEMS
-- ============================================================================
CREATE POLICY "order_items: read own or admin"
  ON order_items FOR SELECT
  USING (
    EXISTS (
      SELECT 1 FROM orders
      WHERE orders.id = order_items.order_id
        AND (orders.user_id = auth.uid() OR is_admin())
    )
  );

CREATE POLICY "order_items: insert authenticated"
  ON order_items FOR INSERT
  WITH CHECK (auth.uid() IS NOT NULL);


-- ============================================================================
-- RLS POLICIES — INVENTORY MOVEMENTS (admin only)
-- ============================================================================
CREATE POLICY "inventory_movements: admin read"
  ON inventory_movements FOR SELECT
  USING (is_admin());

CREATE POLICY "inventory_movements: admin insert"
  ON inventory_movements FOR INSERT
  WITH CHECK (is_admin());


-- ============================================================================
-- RLS POLICIES — CONTACT MESSAGES
-- ============================================================================
CREATE POLICY "contact_messages: anyone can insert"
  ON contact_messages FOR INSERT
  WITH CHECK (true);

CREATE POLICY "contact_messages: admin read"
  ON contact_messages FOR SELECT
  USING (is_admin());

CREATE POLICY "contact_messages: admin update"
  ON contact_messages FOR UPDATE
  USING (is_admin());


-- ============================================================================
-- STORAGE BUCKET — product-images
-- ============================================================================
INSERT INTO storage.buckets (id, name, public)
VALUES ('product-images', 'product-images', true);

-- Public read
CREATE POLICY "Public read product images"
  ON storage.objects FOR SELECT
  USING (bucket_id = 'product-images');

-- Admin write
CREATE POLICY "Admin write product images"
  ON storage.objects FOR INSERT
  WITH CHECK (bucket_id = 'product-images' AND is_admin());

CREATE POLICY "Admin update product images"
  ON storage.objects FOR UPDATE
  USING (bucket_id = 'product-images' AND is_admin());

CREATE POLICY "Admin delete product images"
  ON storage.objects FOR DELETE
  USING (bucket_id = 'product-images' AND is_admin());


-- ============================================================================
-- END OF MIGRATION
-- ============================================================================
