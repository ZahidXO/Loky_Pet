-- ============================================================================
-- LokyPet E-Commerce — Seed Data
-- Generated: 2026-11-06
-- ============================================================================
-- Populates the full LokyPet catalog:
--   • 3 categories
--   • 5 base products
--   • 73 product variants (all with stock = 10)
--   • 5 product images (one per base product)
--
-- Uses CTEs so that IDs are resolved dynamically via gen_random_uuid().
-- Wrapped in a transaction; uses ON CONFLICT DO NOTHING for idempotency.
-- ============================================================================

BEGIN;

-- ============================================================================
-- 1. CATEGORIES (3)
-- ============================================================================
INSERT INTO categories (name, slug, sort_order) VALUES
  ('Camas',    'camas',    1),
  ('Juguetes', 'juguetes', 2),
  ('Cojines',  'cojines',  3)
ON CONFLICT (slug) DO NOTHING;


-- ============================================================================
-- 2. PRODUCTS (5)
-- ============================================================================
INSERT INTO products (category_id, name, slug, description) VALUES
  (
    (SELECT id FROM categories WHERE slug = 'camas'),
    'Cama',
    'cama',
    'Innovador accesorio sostenible y lavable hecho con materiales reciclados que, mediante cierres ocultos, se transforma de un divertido juguete de peluche a una cómoda cama redonda para la mascota.'
  ),
  (
    (SELECT id FROM categories WHERE slug = 'juguetes'),
    'Pelota de tiras',
    'pelota-de-tiras',
    'Fabricada utilizando tiras de camisetas viejas entrelazadas para formar una pelota firme y divertida.'
  ),
  (
    (SELECT id FROM categories WHERE slug = 'juguetes'),
    'Ratón de medias',
    'raton-de-medias',
    'Elaborado utilizando una media vieja como base y rellena con tela reciclada, simulando la forma de un pequeño roedor.'
  ),
  (
    (SELECT id FROM categories WHERE slug = 'juguetes'),
    'Cuerda de juego',
    'cuerda-de-juego',
    'Juguete trenzado hecho con tiras de mezclilla o algodón, ideal para que las mascotas muerdan y tiren de él.'
  ),
  (
    (SELECT id FROM categories WHERE slug = 'cojines'),
    'Cojín rellenable',
    'cojin-rellenable',
    'Pequeño cojín cuadrado hecho con tela resistente (como mezclilla) y relleno con retazos de tela o algodón.'
  )
ON CONFLICT (slug) DO NOTHING;


-- ============================================================================
-- 3. PRODUCT VARIANTS (73 total — all with stock = 10)
-- ============================================================================

-- ---------------------------------------------------------------------------
-- 3a. Cama variants (13) — on sale: compare_at = 199, price = 160
-- ---------------------------------------------------------------------------
INSERT INTO product_variants (product_id, sku, slug, color_name, color_hex, price_mxn, compare_at_price_mxn, stock) VALUES
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-AZU', 'cama-azul',             'Azul',             ARRAY['#2F6FB5'],             160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-ROS', 'cama-rosa',             'Rosa',             ARRAY['#F4A6C1'],             160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-VER', 'cama-verde',            'Verde',            ARRAY['#4CAF50'],             160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-AMA', 'cama-amarillo',         'Amarillo',         ARRAY['#F5D04C'],             160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-MOR', 'cama-morado',           'Morado',           ARRAY['#8E5BB5'],             160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-NEG', 'cama-negro',            'Negro',            ARRAY['#1F1F1F'],             160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-BLA', 'cama-blanco',           'Blanco',           ARRAY['#FFFFFF'],             160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-GRI', 'cama-gris',             'Gris',             ARRAY['#9E9E9E'],             160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-MEZ', 'cama-mezclilla',        'Mezclilla',        ARRAY['#4A6A8A'],             160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-AZB', 'cama-azul-con-blanco',  'Azul con blanco',  ARRAY['#2F6FB5','#FFFFFF'],   160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-RSG', 'cama-rosa-con-gris',    'Rosa con gris',    ARRAY['#F4A6C1','#9E9E9E'],   160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-RSC', 'cama-rosa-con-cafe',    'Rosa con café',    ARRAY['#F4A6C1','#7B5034'],   160, 199, 10),
  ((SELECT id FROM products WHERE slug = 'cama'), 'CAM-NGR', 'cama-negro-con-rojo',   'Negro con rojo',   ARRAY['#1F1F1F','#D32F2F'],   160, 199, 10)
ON CONFLICT (sku) DO NOTHING;

-- ---------------------------------------------------------------------------
-- 3b. Pelota de tiras variants (15) — no sale: price = 50, compare_at = NULL
-- ---------------------------------------------------------------------------
INSERT INTO product_variants (product_id, sku, slug, color_name, color_hex, price_mxn, compare_at_price_mxn, stock) VALUES
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-AZU', 'pelota-de-tiras-azul',             'Azul',              ARRAY['#2F6FB5'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-ROS', 'pelota-de-tiras-rosa',             'Rosa',              ARRAY['#F4A6C1'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-VER', 'pelota-de-tiras-verde',            'Verde',             ARRAY['#4CAF50'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-AMA', 'pelota-de-tiras-amarillo',         'Amarillo',          ARRAY['#F5D04C'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-MOR', 'pelota-de-tiras-morado',           'Morado',            ARRAY['#8E5BB5'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-NEG', 'pelota-de-tiras-negro',            'Negro',             ARRAY['#1F1F1F'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-BLA', 'pelota-de-tiras-blanco',           'Blanco',            ARRAY['#FFFFFF'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-GRI', 'pelota-de-tiras-gris',             'Gris',              ARRAY['#9E9E9E'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-VEB', 'pelota-de-tiras-verde-y-blanco',   'Verde y blanco',    ARRAY['#4CAF50','#FFFFFF'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-AZB', 'pelota-de-tiras-azul-con-blanco',  'Azul con blanco',   ARRAY['#2F6FB5','#FFFFFF'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-AZN', 'pelota-de-tiras-azul-con-negro',   'Azul con negro',    ARRAY['#2F6FB5','#1F1F1F'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-RSC', 'pelota-de-tiras-rosa-con-cafe',    'Rosa con café',     ARRAY['#F4A6C1','#7B5034'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-NGR', 'pelota-de-tiras-negro-con-rojo',   'Negro con rojo',    ARRAY['#1F1F1F','#D32F2F'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-ROJ', 'pelota-de-tiras-rojo',             'Rojo',              ARRAY['#D32F2F'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'), 'PEL-MRS', 'pelota-de-tiras-morado-con-rosa',  'Morado con rosa',   ARRAY['#8E5BB5','#F4A6C1'],   50, NULL, 10)
ON CONFLICT (sku) DO NOTHING;

-- ---------------------------------------------------------------------------
-- 3c. Ratón de medias variants (15) — no sale: price = 50, compare_at = NULL
-- ---------------------------------------------------------------------------
INSERT INTO product_variants (product_id, sku, slug, color_name, color_hex, price_mxn, compare_at_price_mxn, stock) VALUES
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-AZU', 'raton-de-medias-azul',             'Azul',              ARRAY['#2F6FB5'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-ROS', 'raton-de-medias-rosa',             'Rosa',              ARRAY['#F4A6C1'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-VER', 'raton-de-medias-verde',            'Verde',             ARRAY['#4CAF50'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-AMA', 'raton-de-medias-amarillo',         'Amarillo',          ARRAY['#F5D04C'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-MOR', 'raton-de-medias-morado',           'Morado',            ARRAY['#8E5BB5'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-NEG', 'raton-de-medias-negro',            'Negro',             ARRAY['#1F1F1F'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-BLA', 'raton-de-medias-blanco',           'Blanco',            ARRAY['#FFFFFF'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-GRI', 'raton-de-medias-gris',             'Gris',              ARRAY['#9E9E9E'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-VEB', 'raton-de-medias-verde-y-blanco',   'Verde y blanco',    ARRAY['#4CAF50','#FFFFFF'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-AZB', 'raton-de-medias-azul-con-blanco',  'Azul con blanco',   ARRAY['#2F6FB5','#FFFFFF'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-AZN', 'raton-de-medias-azul-con-negro',   'Azul con negro',    ARRAY['#2F6FB5','#1F1F1F'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-RSC', 'raton-de-medias-rosa-con-cafe',    'Rosa con café',     ARRAY['#F4A6C1','#7B5034'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-NGR', 'raton-de-medias-negro-con-rojo',   'Negro con rojo',    ARRAY['#1F1F1F','#D32F2F'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-ROJ', 'raton-de-medias-rojo',             'Rojo',              ARRAY['#D32F2F'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'), 'RAT-MRS', 'raton-de-medias-morado-con-rosa',  'Morado con rosa',   ARRAY['#8E5BB5','#F4A6C1'],   50, NULL, 10)
ON CONFLICT (sku) DO NOTHING;

-- ---------------------------------------------------------------------------
-- 3d. Cuerda de juego variants (15) — no sale: price = 50, compare_at = NULL
-- ---------------------------------------------------------------------------
INSERT INTO product_variants (product_id, sku, slug, color_name, color_hex, price_mxn, compare_at_price_mxn, stock) VALUES
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-AZU', 'cuerda-de-juego-azul',             'Azul',              ARRAY['#2F6FB5'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-ROS', 'cuerda-de-juego-rosa',             'Rosa',              ARRAY['#F4A6C1'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-VER', 'cuerda-de-juego-verde',            'Verde',             ARRAY['#4CAF50'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-AMA', 'cuerda-de-juego-amarillo',         'Amarillo',          ARRAY['#F5D04C'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-MOR', 'cuerda-de-juego-morado',           'Morado',            ARRAY['#8E5BB5'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-NEG', 'cuerda-de-juego-negro',            'Negro',             ARRAY['#1F1F1F'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-BLA', 'cuerda-de-juego-blanco',           'Blanco',            ARRAY['#FFFFFF'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-GRI', 'cuerda-de-juego-gris',             'Gris',              ARRAY['#9E9E9E'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-VEB', 'cuerda-de-juego-verde-y-blanco',   'Verde y blanco',    ARRAY['#4CAF50','#FFFFFF'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-AZB', 'cuerda-de-juego-azul-con-blanco',  'Azul con blanco',   ARRAY['#2F6FB5','#FFFFFF'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-AZN', 'cuerda-de-juego-azul-con-negro',   'Azul con negro',    ARRAY['#2F6FB5','#1F1F1F'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-RSC', 'cuerda-de-juego-rosa-con-cafe',    'Rosa con café',     ARRAY['#F4A6C1','#7B5034'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-NGR', 'cuerda-de-juego-negro-con-rojo',   'Negro con rojo',    ARRAY['#1F1F1F','#D32F2F'],   50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-ROJ', 'cuerda-de-juego-rojo',             'Rojo',              ARRAY['#D32F2F'],             50, NULL, 10),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'), 'CUE-MRS', 'cuerda-de-juego-morado-con-rosa',  'Morado con rosa',   ARRAY['#8E5BB5','#F4A6C1'],   50, NULL, 10)
ON CONFLICT (sku) DO NOTHING;

-- ---------------------------------------------------------------------------
-- 3e. Cojín rellenable variants (15) — on sale: compare_at = 129, price = 50
-- ---------------------------------------------------------------------------
INSERT INTO product_variants (product_id, sku, slug, color_name, color_hex, price_mxn, compare_at_price_mxn, stock) VALUES
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-AZU', 'cojin-rellenable-azul',             'Azul',              ARRAY['#2F6FB5'],             50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-ROS', 'cojin-rellenable-rosa',             'Rosa',              ARRAY['#F4A6C1'],             50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-VER', 'cojin-rellenable-verde',            'Verde',             ARRAY['#4CAF50'],             50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-AMA', 'cojin-rellenable-amarillo',         'Amarillo',          ARRAY['#F5D04C'],             50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-MOR', 'cojin-rellenable-morado',           'Morado',            ARRAY['#8E5BB5'],             50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-NEG', 'cojin-rellenable-negro',            'Negro',             ARRAY['#1F1F1F'],             50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-BLA', 'cojin-rellenable-blanco',           'Blanco',            ARRAY['#FFFFFF'],             50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-GRI', 'cojin-rellenable-gris',             'Gris',              ARRAY['#9E9E9E'],             50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-VEB', 'cojin-rellenable-verde-y-blanco',   'Verde y blanco',    ARRAY['#4CAF50','#FFFFFF'],   50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-AZB', 'cojin-rellenable-azul-con-blanco',  'Azul con blanco',   ARRAY['#2F6FB5','#FFFFFF'],   50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-AZN', 'cojin-rellenable-azul-con-negro',   'Azul con negro',    ARRAY['#2F6FB5','#1F1F1F'],   50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-RSC', 'cojin-rellenable-rosa-con-cafe',    'Rosa con café',     ARRAY['#F4A6C1','#7B5034'],   50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-NGR', 'cojin-rellenable-negro-con-rojo',   'Negro con rojo',    ARRAY['#1F1F1F','#D32F2F'],   50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-ROJ', 'cojin-rellenable-rojo',             'Rojo',              ARRAY['#D32F2F'],             50, 129, 10),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), 'COJ-MRS', 'cojin-rellenable-morado-con-rosa',  'Morado con rosa',   ARRAY['#8E5BB5','#F4A6C1'],   50, 129, 10)
ON CONFLICT (sku) DO NOTHING;


-- ============================================================================
-- 4. PRODUCT IMAGES (1 per base product)
-- ============================================================================
INSERT INTO product_images (product_id, storage_path, sort_order, alt_text) VALUES
  ((SELECT id FROM products WHERE slug = 'cama'),             '/images/products/cama.png',              0, 'Cama para mascota'),
  ((SELECT id FROM products WHERE slug = 'pelota-de-tiras'),  '/images/products/pelota-de-tiras.png',   0, 'Pelota de tiras'),
  ((SELECT id FROM products WHERE slug = 'raton-de-medias'),  '/images/products/raton-de-medias.png',   0, 'Ratón de medias'),
  ((SELECT id FROM products WHERE slug = 'cuerda-de-juego'),  '/images/products/cuerda-de-juego.png',   0, 'Cuerda de juego'),
  ((SELECT id FROM products WHERE slug = 'cojin-rellenable'), '/images/products/cojin-rellenable.png',  0, 'Cojín rellenable')
ON CONFLICT DO NOTHING;


-- ============================================================================
-- DONE
-- ============================================================================
COMMIT;
