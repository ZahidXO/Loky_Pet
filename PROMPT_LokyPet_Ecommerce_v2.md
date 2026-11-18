# PROMPT MAESTRO — E-commerce "LOKY PET" (Blazor WebAssembly + DaisyUI + Supabase + Vercel)

> **Instrucciones para la IA que recibe este prompt:** lee el documento completo antes de escribir código. Este documento es la **fuente única de verdad** del proyecto. Respeta al pie de la letra la **Regla de Control** (sección 0), el **Sistema de diseño** (sección 4, prioridad alta), las secciones marcadas como **[TEXTO LITERAL]** (sección 10) y la lista de **Decisiones** (sección 15).

---

## 0. Regla de Control (obligatoria, prioridad máxima)

1. **Yo (el usuario) dirijo el proyecto.** Tú me ayudas a desarrollarlo, completar detalles faltantes y terminar lo que quedó inconcluso, pero **jamás tomas el control**.
2. **Prohibido agregar funcionalidades, pantallas, dependencias, textos, categorías o datos que no estén en este documento** sin antes:
   - explicarme **qué** quieres agregar,
   - **por qué** lo propones (beneficio concreto),
   - **qué impacto** tiene (archivos, tiempo, dependencias),
   - y **esperar mi autorización explícita** ("sí, agrégalo").
3. Si algo es ambiguo o falta información, **pregunta antes de inventar**. Nunca rellenes huecos de precios, stock o datos legales con información inventada sin decírmelo.
4. Trabaja **por fases** (sección 14). Al terminar cada fase, detente, muéstrame un resumen de lo hecho y espera mi confirmación antes de continuar.
5. Todo el texto visible al usuario final va en **español de México**. Código, clases, carpetas y tablas en **inglés** (convención .NET).

### 0.1 Fuera de alcance (NO implementar)
- **Tallas, guías de tallas y medidas de producto**: los productos ya están definidos y no las necesitan. No crear campos, tablas, componentes ni secciones para ellas.
- **Categorías Trajes, Collares y Alimento**: no existen en el catálogo. No mostrarlas ni como "Próximamente".
- **WhatsApp**: por ahora solo habrá dos redes sociales (Facebook e Instagram).
- **Pasarelas de pago reales**: el pago es 100 % simulado.
- Lista de deseos, reseñas, cupones, contadores de "oferta relámpago" con cuenta regresiva, chat en vivo, notificaciones push: **no** se implementan salvo que yo lo autorice.

---

## 1. Idea clave del proyecto

Construir una aplicación web de **e-commerce de accesorios para mascotas** para **LOKY PET, S.A. DE C.V.**, una empresa de Reynosa, Tamaulipas, que confecciona productos para mascotas mediante **upcycling textil** (reciclaje y reutilización creativa de telas).

- **Inspiración visual:** la experiencia de compra de **SHEIN**, adaptada a la identidad de LOKY PET y a la paleta "Neapolitan". La interfaz debe ser **novedosa, limpia, estética y muy amigable** (ver sección 4; es uno de los requisitos más importantes del proyecto).
- **Dos módulos principales:**
  1. **Tienda pública** (clientes): ver, explorar y "comprar" productos.
  2. **Dashboard** (vendedores/administradores): ver métricas y administrar productos e inventario.
- **Módulo adicional:** **Mi Cuenta** del cliente (perfil + avatar).
- **El pago NO es real.** Se simula una transacción completa (formulario, validación, "procesando…", confirmación y número de pedido) sin conectarse a ninguna pasarela ni cobrar nada.

---

## 2. Stack tecnológico (obligatorio, no sustituir)

| Capa | Tecnología | Notas |
|---|---|---|
| Frontend | **Blazor WebAssembly standalone** sobre **.NET 8 (LTS)** o superior | SPA 100 % cliente, sin servidor ASP.NET propio. Toda la lógica en C#. |
| Estilos | **Tailwind CSS + DaisyUI** | Compilar Tailwind con su CLI (vía npm) escaneando `.razor`, `.cs` y `.html`. Tema DaisyUI personalizado `lokypet` (sección 4). Eliminar el Bootstrap de la plantilla de Blazor. |
| Iconos | **Font Awesome** (free) | Para reproducir los íconos de las referencias (pata, ubicación, sobre, teléfono, carpeta, edificios, mapa, flecha de retorno, caja, tarjeta, "R" registrada, camión, escudo, hoja, calendario) y los logotipos de Facebook e Instagram. |
| Base de datos / Auth / Archivos | **Supabase** (PostgreSQL + Auth + Storage) | Cliente oficial de C# (NuGet `Supabase`). Seguridad con **Row Level Security (RLS)**: la `anon key` es pública porque la app corre en el navegador. **Nunca** incluir la `service_role key` en el cliente. |
| Despliegue | **Vercel** | Sitio estático (sección 13). |
| Animaciones | CSS/Tailwind (transitions, keyframes) + JS interop mínimo si es indispensable | Sin librerías pesadas de animación salvo autorización. |

---

## 3. Arquitectura (Clean Architecture)

```
LokyPet.sln
├── src/
│   ├── LokyPet.Domain/            → Entidades, Value Objects, Enums, reglas de negocio puras. Sin dependencias externas.
│   │     Entities/ (Category, Product, ProductVariant, ProductImage, Order, OrderItem,
│   │                UserProfile, ContactMessage, InventoryMovement)
│   │     Enums/ (OrderStatus, UserRole, AvatarBreed, StockStatus, ShippingType)
│   │     ValueObjects/ (Money [MXN], Sku, ColorSwatch, Discount)
│   │     Policies/ (ShippingPolicy: local Reynosa $0 · fuera de Reynosa $139 por DHL)
│   ├── LokyPet.Application/       → Casos de uso, interfaces (puertos), DTOs, validaciones, mapeos.
│   │     Interfaces/ (IProductRepository, IOrderRepository, ICartService, IAuthService,
│   │                  IStorageService, IContactService, IDashboardMetricsService)
│   │     UseCases/ (Catalog, Cart, Checkout, Inventory, Dashboard, Account, Contact)
│   │     DTOs/  Validators/
│   ├── LokyPet.Infrastructure/    → Implementaciones con Supabase (repositorios, auth, storage) y LocalStorage del carrito.
│   └── LokyPet.Client/            → Proyecto Blazor WASM (presentación).
│         Layouts/ (StoreLayout, DashboardLayout, AccountLayout)
│         Pages/Store/ (Home, Accessories, ProductDetail, Cart, Checkout, OrderConfirmation, About, Contact)
│         Pages/Dashboard/ (Overview, Inventory, ProductEditor, Orders, Messages)
│         Pages/Account/ (Login, Register, Profile)
│         Components/ (ProductCard, PriceTag, DiscountBadge, ColorSwatch, Navbar, AnnouncementBar,
│                      CartDrawer, HeroCarousel, CategoryChips, Accordion, Tabs, SocialLinks, Footer, Skeletons, Charts…)
│         wwwroot/ (css/app.css compilado, images/products, images/avatars, images/brand)
├── tests/
│   ├── LokyPet.Domain.Tests/
│   └── LokyPet.Application.Tests/
└── supabase/
    ├── migrations/  (tablas, RLS, políticas de Storage, funciones RPC)
    └── seed.sql     (catálogo de la sección 8)
```

Reglas:
- Las dependencias apuntan **hacia adentro**: Client → Application → Domain; Infrastructure → Application/Domain.
- Las páginas Razor **no** hablan directamente con Supabase: siempre a través de interfaces de Application.
- Inyección de dependencias en `Program.cs`.
- Configuración (URL y `anon key` de Supabase, **enlaces de redes sociales**, tarifa de envío) en `wwwroot/appsettings.json`, nunca hardcodeada en componentes.
- Estado global (carrito, usuario) con servicios que emiten eventos `OnChange` para refrescar la UI.

---

## 4. Sistema de diseño UI/UX — PRIORIDAD ALTA

> El diseño es **sumamente importante**. La interfaz debe sentirse **novedosa, limpia, basada en SHEIN, lo más estética posible y muy amigable para el usuario**. Un sitio funcional pero visualmente genérico **no** cumple con este proyecto.

### 4.1 Principios
1. **El producto es el protagonista**: imágenes grandes, fondos limpios, texto mínimo en tarjetas.
2. **Limpieza**: mucho aire (espaciado generoso), jerarquía visual clara, máximo **una acción principal** por bloque.
3. **Compra sin fricción**: de la portada al pago en el menor número de pasos; "Agregar al carrito" siempre visible.
4. **Calidez de marca**: estética suave tipo "boutique" con la paleta Neapolitan; detalles sutiles de mascotas (patitas, curvas).
5. **Consistencia**: mismos radios, sombras, espaciados y animaciones en todo el sitio.
6. **Mobile-first**: la mayoría de las compras tipo SHEIN son en el celular; diseñar primero para 360–430 px.

### 4.2 Patrones de SHEIN que se deben replicar (adaptados)
- **Barra de anuncios** superior delgada con mensajes rotativos (fade cada 4 s).
- **Header** con logotipo, **buscador prominente** con placeholder animado ("Busca camas, pelotas, cojines…"), íconos de cuenta y carrito con contador.
- **Chips de categorías** con scroll horizontal (Todos · Camas · Juguetes · Cojines).
- **Banners a todo lo ancho** con fotografía de producto, titular corto y CTA.
- **Grid denso de tarjetas** sin bordes duros, imagen arriba, precio destacado, **precio anterior tachado** e **insignia de descuento** ("-20%", "-61%") en la esquina.
- **Filtros sticky** (barra superior en móvil que abre un *bottom sheet*; panel lateral en escritorio).
- **Mini-carrito lateral (drawer)** al agregar un producto.
- **Barra inferior de navegación en móvil** (Home, Accesorios, Carrito, Sobre nosotros, Cuenta) y **barra "Agregar al carrito" fija** en la parte inferior del detalle de producto.
- **Recomendaciones** al final de detalle y carrito ("También te puede interesar").

### 4.3 Toques novedosos (lo que distingue a LOKY PET)
- **Navbar "glass"**: fondo crema translúcido con `backdrop-blur`, que se compacta (reduce altura y sombra) al hacer scroll.
- **Indicador píldora animado** que se desliza hacia el submódulo activo.
- **Transición entre submódulos**: el contenido sale con *fade + translateY(8px)* y entra con *fade + translateY(0)* (250–350 ms).
- **Animación "agregar al carrito"**: una miniatura del producto "vuela" hacia el ícono del carrito y el contador hace un pequeño *bounce*; si el usuario tiene `prefers-reduced-motion`, solo se actualiza el contador.
- **Muestras de color** circulares; las bicolores se muestran como un círculo dividido en diagonal. Al pasar el cursor, la muestra crece 10 % con un anillo café.
- **Separadores con curvas suaves** (ondas SVG inspiradas en las líneas de la lámina Neapolitan) entre secciones del Home y Sobre nosotros.
- **Patitas decorativas** muy sutiles (opacidad 5–8 %) en fondos de secciones vacías o hero; nunca sobre texto.
- **Estados vacíos ilustrados** (carrito vacío, sin resultados) con un ícono de mascota y un mensaje amable, p. ej. "Tu carrito está esperando a su primer juguete 🐾".
- **Skeleton loaders** con brillo suave (shimmer rosa) mientras carga Supabase, nunca *spinners* a pantalla completa.

### 4.4 Tokens de diseño

**Color — paleta oficial "Neapolitan" (obligatoria):**

| Token DaisyUI | Hex | Uso |
|---|---|---|
| `primary` | **#604734** (café chocolate) | Botones principales, texto fuerte, precio final, navbar activa, footer |
| `base-100` | **#fcf7d9** (crema vainilla) | Fondo general |
| `secondary` | **#f6d8d6** (rosa pastel) | Fondos de tarjetas, secciones suaves, chips inactivos |
| `accent` | **#f0c5d4** (rosa fresa) | Insignias de oferta, hover, indicador activo, detalles |

- Regla 60-30-10: ~60 % crema, ~30 % rosas, ~10 % café.
- Deriva `neutral`, `base-200`, `base-300`, `info`, `success`, `warning`, `error` en tonos armónicos y **muéstrame la propuesta antes de fijarla**.
- Texto principal café #604734 sobre crema #fcf7d9 (contraste WCAG AA). Texto sobre botones café: crema.
- Fuera de la paleta solo se permiten: los hex de muestras de color de producto y los colores de estado (éxito/error).
- Las referencias legales originales usan verde: **conserva estructura y texto, pero con colores `lokypet`**.

**Tipografía** (propón las fuentes exactas y espera aprobación):
- Script caligráfica (estilo del título "Neapolitan") **solo** para el logotipo y 1–2 titulares decorativos.
- Sans-serif limpia (p. ej. *Inter* o *Poppins*) para todo lo demás.
- Escala: 12 / 14 / 16 (cuerpo) / 18 / 24 / 32 / 48 px. Precios en peso semibold.

**Forma y profundidad:**
- Radios: tarjetas y modales `rounded-2xl`; botones e inputs `rounded-full` o `rounded-xl` (elige uno y sé consistente); chips `rounded-full`.
- Sombras suaves y difusas con tinte café (p. ej. `0 8px 24px rgba(96,71,52,.08)`); prohibidas las sombras duras/negras.
- Espaciado en múltiplos de 4 px; contenedor máximo 1280–1440 px.

**Movimiento:**
- Duraciones: 150 ms (hover), 250 ms (componentes), 350 ms (transiciones de página).
- Easing: `cubic-bezier(0.22, 1, 0.36, 1)`.
- Respetar siempre `prefers-reduced-motion`.

### 4.5 Componentes clave
- **ProductCard**: imagen en relación de aspecto fija (1:1) sobre fondo `secondary` muy claro con `object-contain` y padding (las imágenes de referencia tienen tamaños y proporciones distintas y algunas son pequeñas: **no estirarlas ni pixelarlas**); insignia de descuento arriba a la izquierda; nombre en 2 líneas máx.; precio final en café semibold + precio anterior tachado en gris; muestra de color; botón circular "+" para agregar rápido. Hover: elevación suave y zoom 1.05 de la imagen.
- **Botones**: primario (café, texto crema), secundario (contorno café), terciario (texto). Altura mínima 44 px.
- **Insignias**: "-20%" / "-61%" en `accent` con texto café; "Agotado" en gris; "Envío gratis en Reynosa" como etiqueta informativa.
- **Inputs**: grandes, con etiqueta flotante, foco con anillo `accent`, mensajes de error amables debajo del campo.
- **Toasts** en la esquina inferior (móvil: parte superior), con ícono y cierre automático a 3 s.

### 4.6 Responsive
- Breakpoints Tailwind: `sm` 640, `md` 768, `lg` 1024, `xl` 1280.
- Grid de productos: 2 columnas móvil, 3 tablet, 4 escritorio, 5 en pantallas ≥1440 px.
- Áreas táctiles ≥ 44×44 px.

### 4.7 Entregable de diseño
Al final de la **Fase 1**, antes de construir pantallas reales, muéstrame: el tema `lokypet` aplicado (colores derivados, fuentes, botones, tarjeta de producto, chips, inputs, insignias) para que lo apruebe.

---

## 5. MÓDULO 1 — Tienda pública

### 5.1 Navbar
- Sutil, moderna y novedosa, fija arriba (*sticky*) y con el estilo "glass" de 4.3.
- Elementos: logotipo · enlaces a los 5 submódulos (**Home, Accesorios, Carrito, Sobre nosotros, Contacto**) · buscador · carrito con contador animado · avatar de **Mi Cuenta**.
- **Indicador animado** del submódulo activo y **animación en cada cambio de submódulo** (4.3).
- Móvil: barra inferior de navegación (4.2) + header compacto con buscador.
- **Barra de anuncios** con mensajes rotativos (solo datos reales):
  - "🐾 Envío GRATIS en Reynosa · Entrega en 24 a 48 horas hábiles"
  - "📦 Envíos a todo México por DHL · $139.00 MXN"
  - "🔥 Ofertas: Camas a $160 (antes $199) · Cojines a $50 (antes $129)"

### 5.2 Submódulo HOME
1. **Hero/banner carrusel** (3 diapositivas, autoplay 5 s con pausa al pasar el cursor, indicadores tipo píldora):
   - "Camas que se transforman" — oferta Cama **$160** ~~$199~~ → CTA "Ver camas".
   - "Cojines a solo $50" ~~$129~~ → CTA "Ver cojines".
   - "Hecho en Reynosa con textiles reciclados" → CTA "Conócenos".
2. **Ofertas**: carrusel horizontal con los **28 ítems en oferta** (13 camas + 15 cojines), con insignias "-20%" y "-61%" y precio anterior tachado. Botón "Ver todas las ofertas" → Accesorios filtrado por "En oferta".
3. **Compra por categoría**: círculos grandes con imagen de Camas, Juguetes y Cojines.
4. **Propuesta de valor** (4 íconos + texto corto): Hecho en Reynosa · Upcycling textil · Envío gratis en Reynosa (24–48 h) · Envíos a todo México por DHL (3–5 días hábiles).
5. **Novedades / Destacados**: grid de `ProductCard` (8 ítems, mezclando categorías y colores).
6. Footer global (10.1).

### 5.3 Submódulo ACCESORIOS (catálogo)
- Muestra **todos** los ítems. **Cada color es un ítem independiente con su propia tarjeta** (sección 8), para que el inventario se vea lleno.
- Chips de categoría: **Todos · Camas · Juguetes · Cojines** + chip especial **"En oferta"**.
- Filtros: categoría, color (muestras clicables), rango de precio, disponibilidad, solo ofertas.
- Orden: recomendados, mayor descuento, precio menor→mayor, precio mayor→menor.
- Contador de resultados ("73 productos") y chips de filtros activos con "x" para quitarlos.
- Ítems sin stock: insignia **"Agotado"** y botón deshabilitado (no se ocultan, salvo que el dashboard los desactive).

### 5.4 DETALLE DE PRODUCTO (`/accesorios/{slug}`)
Cada ítem tiene su propia página con:
- **Galería** (imagen principal + miniaturas, zoom/lightbox). Escritorio: galería a la izquierda, información a la derecha (sticky).
- Breadcrumb (Inicio › Accesorios › Categoría), nombre, SKU.
- **Precio**: precio final grande; si hay oferta, precio anterior tachado, insignia de % y texto "Ahorras $39.00" (Cama) / "Ahorras $79.00" (Cojín). Leyenda "IVA incluido".
- **Selector de color**: muestras de todos los colores del mismo producto base; al hacer clic navega al slug de ese color sin recargar la página (transición suave).
- **Descripción** (literal del Excel) y bloque "Detalles": categoría, color, "Hecho con textiles reciclados en Reynosa, Tamaulipas".
- Cantidad (limitada al stock) y **"Agregar al carrito"** (barra fija inferior en móvil).
- **Bloque de envío**: "Envío gratis en Reynosa (24 a 48 horas hábiles)" · "Resto de México: $139.00 MXN por DHL (3 a 5 días hábiles)" · "Devolución hasta 10 días naturales por defectos de confección o empaque" con enlace a Sobre nosotros › Envíos y Devoluciones.
- **Productos similares**: carrusel con otros colores del mismo producto y productos de la misma categoría.
- ❌ Sin tallas, guías de tallas ni medidas (0.1).

### 5.5 Submódulo CARRITO
- Cada accesorio agregado: imagen, nombre, color, precio unitario (con precio anterior tachado si está en oferta), cantidad editable, subtotal por línea, botón eliminar (con opción "Deshacer" en el toast).
- **Resumen**:
  - Subtotal
  - **Ahorro por ofertas** (en `accent`, p. ej. "Ahorras $118.00")
  - Envío: "Gratis en Reynosa · $139.00 fuera de Reynosa (DHL) — se confirma en el pago"
  - **Total a pagar** en MXN · leyenda "Precios en MXN, IVA incluido"
- Botón **"Proceder al pago"** (sticky en móvil).
- **"También te puede interesar"**: **6 productos** (4 en móvil), priorizando ofertas y la misma categoría.
- Estado vacío ilustrado con botón "Explorar accesorios".
- Persistencia en `localStorage` (funciona sin sesión).
- Además, **mini-carrito drawer** al agregar un producto.

### 5.6 PAGO (simulado)
- Indicador de pasos: **1. Envío → 2. Pago → 3. Confirmación**.
- **Paso 1 – Envío**: nombre, teléfono, correo, dirección, C.P., ciudad, estado. Tipo de entrega (tarjetas seleccionables):
  - **Entrega local Reynosa — GRATIS** · 24 a 48 horas hábiles. Solo disponible si ciudad = Reynosa y estado = Tamaulipas.
  - **Envío nacional por DHL — $139.00 MXN** · 3 a 5 días hábiles. Para cualquier destino fuera de Reynosa. Mostrar logotipo/texto "DHL".
  - El costo de envío es **por pedido** (no por pieza) y se recalcula en vivo en el resumen.
- **Paso 2 – Pago** (visual, ficticio): tarjeta de crédito/débito (con vista previa animada de la tarjeta), Mercado Pago, efectivo en tiendas de conveniencia. Validación de formato (Luhn, vigencia, CVV) **sin guardar datos de tarjeta en ningún lugar**.
- **Paso 3 – Confirmación**: animación "Procesando pago…" (2–3 s) → pantalla de éxito con animación ligera (check + patitas), número de pedido (`LKP-2026-000123`), resumen, tipo de envío/paquetería y fecha estimada (24 h de procesamiento + tiempo de entrega en días hábiles).
- Se registra el pedido (`orders`, `order_items`) con estado "Pagado (simulado)", `shipping_cost` (0 o 139), `shipping_carrier` (null o "DHL") y **se descuenta stock**.
- Aviso visible: **"Modo demostración: no se realizará ningún cargo real."**
- Compra como invitado permitida; al final se sugiere crear cuenta.

### 5.7 Submódulo SOBRE NOSOTROS
Orden de la página:
1. **Hero de marca** (logotipo + frase breve + imagen de productos).
2. **Historia, Misión, Visión y Valores** (10.6).
3. **Centro de Transparencia Legal** (10.2) con sus 4 pestañas y el contenido asignado a cada una (10.3).
4. Footer corporativo (10.1).

El hero del Centro de Transparencia y las pestañas deben poder abrirse directamente con un parámetro de URL (`/sobre-nosotros?tab=terminos|privacidad|envios|sustentabilidad`), con scroll suave hasta la sección.

### 5.8 Submódulo CONTACTO
- Formulario: nombre, correo, teléfono (opcional), asunto (Pedido, Producto, Envíos y devoluciones, Facturación, Otro), mensaje, casilla "He leído el Aviso de Privacidad" (enlace a la pestaña).
- Validación en cliente; se guarda en `contact_messages`; toast de éxito.
- Datos de contacto corporativos (los de 10.1).
- **Redes sociales (solo dos):** tarjetas/botones con ícono, nombre del perfil e hipervínculo. Por ahora los enlaces apuntan al **inicio de sesión** de cada plataforma como ejemplo; cuando te comparta los enlaces reales solo se cambiará la configuración:

| Red | Texto visible | Enlace temporal (placeholder) |
|---|---|---|
| Instagram (`fa-brands fa-instagram`) | **@lokypet_** | `https://www.instagram.com/accounts/login/` |
| Facebook (`fa-brands fa-facebook`) | **Loky Pet** | `https://www.facebook.com/login/` |

  - Abrir en pestaña nueva (`target="_blank" rel="noopener noreferrer"`), con animación al pasar el cursor (elevación + color de marca de la red).
  - Los enlaces se leen de `appsettings.json`:
    ```json
    "SocialLinks": {
      "Instagram": { "Handle": "@lokypet_", "Url": "https://www.instagram.com/accounts/login/" },
      "Facebook":  { "Handle": "Loky Pet", "Url": "https://www.facebook.com/login/" }
    }
    ```
  - Sin WhatsApp.

---

## 6. MÓDULO 2 — Mi Cuenta (cliente)

- Registro, inicio de sesión y recuperación de contraseña con **Supabase Auth** (correo + contraseña).
- Perfil: nombre, correo, teléfono, dirección de envío predeterminada.
- **Selección de avatar**: 1 de **6 íconos** de caras de perros **realistas y sonriendo**, de razas muy distintas:
  1. **Golden Retriever** (pelaje dorado largo)
  2. **Pug** (cara chata, beige con máscara negra)
  3. **Husky Siberiano** (gris/blanco, ojos azules)
  4. **Dálmata** (blanco con manchas negras)
  5. **Chihuahua** (pequeño, orejas grandes, color canela)
  6. **Shiba Inu** (rojizo/crema, aspecto de zorro)
- Imágenes cuadradas 512×512 (WebP) en `wwwroot/images/avatars/`, fondo crema o rosa de la paleta, mostradas en círculo. Si no puedes generarlas, crea *placeholders* con estos nombres exactos (`golden-retriever.webp`, `pug.webp`, `husky.webp`, `dalmata.webp`, `chihuahua.webp`, `shiba-inu.webp`) y dame el *prompt* de imagen para cada una.
- Selector en grid de 3×2 con animación de selección (anillo `accent` + check).
- El avatar se guarda en `user_profiles.avatar_key` y se muestra en la navbar.
- **Historial de pedidos** (simulados) con detalle, tipo de envío y estado.

---

## 7. MÓDULO 3 — Dashboard (vendedores / administradores)

Acceso **solo** para `role = 'admin'` (protegido por RLS). Ruta `/dashboard`, layout propio (sidebar colapsable + header), mismo tema visual, limpio y legible.

### 7.1 Submódulo principal: Resumen y gráficas de ventas
- KPIs: ventas totales (MXN), número de pedidos, ticket promedio, piezas vendidas, ingresos por envío DHL, pedidos locales vs nacionales, ítems con stock bajo, ítems agotados.
- Gráficas: ventas por día/semana/mes (línea), ventas por categoría (dona), top 10 ítems (barras), ventas por color (barras), ventas con oferta vs sin oferta.
- Filtro por rango de fechas. Alertas de stock bajo (umbral configurable, por defecto ≤ 3).
- Librería de gráficas: **propón una** compatible con Blazor WASM y espera aprobación.

### 7.2 Submódulo: Control de inventario y productos
- Tabla de ítems con búsqueda, filtros (categoría, color, estado de stock, en oferta) y paginación.
- **Crear** producto base y sus variantes de color.
- **Editar**: nombre, descripción, categoría, **precio final**, **precio anterior (para ofertas)**, activar/desactivar oferta, colores/hex, **imágenes** (subir a Supabase Storage, reordenar, eliminar) y disponibilidad. Opción de aplicar el precio/oferta a todas las variantes del producto base a la vez.
- **Editar inventario**: entradas/salidas con motivo, registradas en `inventory_movements`.
- **Marcar fuera de inventario** (agotado) o **desactivar** (oculto en tienda) sin borrar; eliminación lógica con confirmación.
- Vista previa en vivo de la `ProductCard`.

### 7.3 Submódulos adicionales (autorizados en mi descripción: "si crees que es necesario algún otro agrégalo")
- **Pedidos**: lista y detalle de pedidos simulados; cambio de estado (Pagado → En preparación → Enviado → Entregado / Cancelado); campo de número de guía DHL para envíos nacionales.
- **Mensajes de contacto**: bandeja del formulario de Contacto; marcar como leído/respondido.
- Cualquier otro submódulo **requiere mi autorización**.

---

## 8. Catálogo de productos (datos semilla — fuente: archivo Excel)

### 8.1 Reglas del catálogo
- **Un ítem por cada color** de cada producto, con SKU, slug, página y stock propios.
- Modelo: **`Product`** (producto base: nombre, descripción, categoría, precios) → **`ProductVariant`** (ítem por color: SKU, slug, color, hex, stock, imágenes, activo/agotado). El catálogo muestra **variantes**.
- **Precios en MXN, IVA incluido.**
- **Descripción literal del Excel** (no modificarla).
- **Imágenes**: 1 imagen de referencia por producto base (carpeta `imagenes_productos`), usada en todas sus variantes hasta que se suban fotos por color desde el dashboard. **No** teñirlas ni alterarlas para simular colores.
- Los hex son **solo para las muestras de color** de la interfaz.
- **Stock inicial**: 10 piezas por ítem (valor de demostración; se edita en el dashboard).
- Correcciones aplicadas al Excel: "grus" → **Gris**; "azul con blanca" → **Azul con blanco**; espacios sobrantes eliminados y colores capitalizados.

### 8.2 Productos base y ofertas

| Prefijo | Producto | Categoría | Colores | Precio anterior | **Precio final** | Descuento | Imagen | Descripción (literal del Excel) |
|---|---|---|---|---|---|---|---|---|
| CAM | Cama | Camas | 13 | ~~$199.00~~ | **$160.00** | **-20 %** (ahorro $39.00) | `cama.png` | Innovador accesorio sostenible y lavable hecho con materiales reciclados que, mediante cierres ocultos, se transforma de un divertido juguete de peluche a una cómoda cama redonda para la mascota. |
| PEL | Pelota de tiras | Juguetes | 15 | — | **$50.00** | — | `pelota-de-tiras.png` | Fabricada utilizando tiras de camisetas viejas entrelazadas para formar una pelota firme y divertida. |
| RAT | Ratón de medias | Juguetes | 15 | — | **$50.00** | — | `raton-de-medias.png` | Elaborado utilizando una media vieja como base y rellena con tela reciclada, simulando la forma de un pequeño roedor. |
| CUE | Cuerda de juego | Juguetes | 15 | — | **$50.00** | — | `cuerda-de-juego.png` | Juguete trenzado hecho con tiras de mezclilla o algodón, ideal para que las mascotas muerdan y tiren de él. |
| COJ | Cojín rellenable | Cojines | 15 | ~~$129.00~~ | **$50.00** | **-61 %** (ahorro $79.00) | `cojin-rellenable.png` | Pequeño cojín cuadrado hecho con tela resistente (como mezclilla) y relleno con retazos de tela o algodón. |

- **Total: 5 productos base → 73 ítems**, de los cuales **28 están en oferta** (13 camas + 15 cojines).
- El **precio final** es el que se cobra; el **precio anterior** solo se muestra tachado como referencia de la oferta.
- Porcentaje de descuento = `round((1 − final / anterior) × 100)`: Cama 19.6 % → **-20 %**; Cojín 61.2 % → **-61 %**.
- Las ofertas no tienen fecha de término (vigentes hasta que se desactiven desde el dashboard). **No** mostrar cuentas regresivas.

### 8.3 Listado completo de ítems (73)

| # | SKU | Nombre del ítem | Categoría | Slug | Color | Hex (muestra) | Precio anterior | Precio final | Oferta |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `CAM-AZU` | Cama – Azul | Camas | `cama-azul` | Azul | #2F6FB5 | ~~$199.00~~ | **$160.00** | -20% |
| 2 | `CAM-ROS` | Cama – Rosa | Camas | `cama-rosa` | Rosa | #F4A6C1 | ~~$199.00~~ | **$160.00** | -20% |
| 3 | `CAM-VER` | Cama – Verde | Camas | `cama-verde` | Verde | #4CAF50 | ~~$199.00~~ | **$160.00** | -20% |
| 4 | `CAM-AMA` | Cama – Amarillo | Camas | `cama-amarillo` | Amarillo | #F5D04C | ~~$199.00~~ | **$160.00** | -20% |
| 5 | `CAM-MOR` | Cama – Morado | Camas | `cama-morado` | Morado | #8E5BB5 | ~~$199.00~~ | **$160.00** | -20% |
| 6 | `CAM-NEG` | Cama – Negro | Camas | `cama-negro` | Negro | #1F1F1F | ~~$199.00~~ | **$160.00** | -20% |
| 7 | `CAM-BLA` | Cama – Blanco | Camas | `cama-blanco` | Blanco | #FFFFFF | ~~$199.00~~ | **$160.00** | -20% |
| 8 | `CAM-GRI` | Cama – Gris | Camas | `cama-gris` | Gris | #9E9E9E | ~~$199.00~~ | **$160.00** | -20% |
| 9 | `CAM-MEZ` | Cama – Mezclilla | Camas | `cama-mezclilla` | Mezclilla | #4A6A8A | ~~$199.00~~ | **$160.00** | -20% |
| 10 | `CAM-AZB` | Cama – Azul con blanco | Camas | `cama-azul-con-blanco` | Azul con blanco | #2F6FB5 + #FFFFFF | ~~$199.00~~ | **$160.00** | -20% |
| 11 | `CAM-RSG` | Cama – Rosa con gris | Camas | `cama-rosa-con-gris` | Rosa con gris | #F4A6C1 + #9E9E9E | ~~$199.00~~ | **$160.00** | -20% |
| 12 | `CAM-RSC` | Cama – Rosa con café | Camas | `cama-rosa-con-cafe` | Rosa con café | #F4A6C1 + #7B5034 | ~~$199.00~~ | **$160.00** | -20% |
| 13 | `CAM-NGR` | Cama – Negro con rojo | Camas | `cama-negro-con-rojo` | Negro con rojo | #1F1F1F + #D32F2F | ~~$199.00~~ | **$160.00** | -20% |
| 14 | `PEL-AZU` | Pelota de tiras – Azul | Juguetes | `pelota-de-tiras-azul` | Azul | #2F6FB5 | — | **$50.00** | — |
| 15 | `PEL-ROS` | Pelota de tiras – Rosa | Juguetes | `pelota-de-tiras-rosa` | Rosa | #F4A6C1 | — | **$50.00** | — |
| 16 | `PEL-VER` | Pelota de tiras – Verde | Juguetes | `pelota-de-tiras-verde` | Verde | #4CAF50 | — | **$50.00** | — |
| 17 | `PEL-AMA` | Pelota de tiras – Amarillo | Juguetes | `pelota-de-tiras-amarillo` | Amarillo | #F5D04C | — | **$50.00** | — |
| 18 | `PEL-MOR` | Pelota de tiras – Morado | Juguetes | `pelota-de-tiras-morado` | Morado | #8E5BB5 | — | **$50.00** | — |
| 19 | `PEL-NEG` | Pelota de tiras – Negro | Juguetes | `pelota-de-tiras-negro` | Negro | #1F1F1F | — | **$50.00** | — |
| 20 | `PEL-BLA` | Pelota de tiras – Blanco | Juguetes | `pelota-de-tiras-blanco` | Blanco | #FFFFFF | — | **$50.00** | — |
| 21 | `PEL-GRI` | Pelota de tiras – Gris | Juguetes | `pelota-de-tiras-gris` | Gris | #9E9E9E | — | **$50.00** | — |
| 22 | `PEL-VEB` | Pelota de tiras – Verde y blanco | Juguetes | `pelota-de-tiras-verde-y-blanco` | Verde y blanco | #4CAF50 + #FFFFFF | — | **$50.00** | — |
| 23 | `PEL-AZB` | Pelota de tiras – Azul con blanco | Juguetes | `pelota-de-tiras-azul-con-blanco` | Azul con blanco | #2F6FB5 + #FFFFFF | — | **$50.00** | — |
| 24 | `PEL-AZN` | Pelota de tiras – Azul con negro | Juguetes | `pelota-de-tiras-azul-con-negro` | Azul con negro | #2F6FB5 + #1F1F1F | — | **$50.00** | — |
| 25 | `PEL-RSC` | Pelota de tiras – Rosa con café | Juguetes | `pelota-de-tiras-rosa-con-cafe` | Rosa con café | #F4A6C1 + #7B5034 | — | **$50.00** | — |
| 26 | `PEL-NGR` | Pelota de tiras – Negro con rojo | Juguetes | `pelota-de-tiras-negro-con-rojo` | Negro con rojo | #1F1F1F + #D32F2F | — | **$50.00** | — |
| 27 | `PEL-ROJ` | Pelota de tiras – Rojo | Juguetes | `pelota-de-tiras-rojo` | Rojo | #D32F2F | — | **$50.00** | — |
| 28 | `PEL-MRS` | Pelota de tiras – Morado con rosa | Juguetes | `pelota-de-tiras-morado-con-rosa` | Morado con rosa | #8E5BB5 + #F4A6C1 | — | **$50.00** | — |
| 29 | `RAT-AZU` | Ratón de medias – Azul | Juguetes | `raton-de-medias-azul` | Azul | #2F6FB5 | — | **$50.00** | — |
| 30 | `RAT-ROS` | Ratón de medias – Rosa | Juguetes | `raton-de-medias-rosa` | Rosa | #F4A6C1 | — | **$50.00** | — |
| 31 | `RAT-VER` | Ratón de medias – Verde | Juguetes | `raton-de-medias-verde` | Verde | #4CAF50 | — | **$50.00** | — |
| 32 | `RAT-AMA` | Ratón de medias – Amarillo | Juguetes | `raton-de-medias-amarillo` | Amarillo | #F5D04C | — | **$50.00** | — |
| 33 | `RAT-MOR` | Ratón de medias – Morado | Juguetes | `raton-de-medias-morado` | Morado | #8E5BB5 | — | **$50.00** | — |
| 34 | `RAT-NEG` | Ratón de medias – Negro | Juguetes | `raton-de-medias-negro` | Negro | #1F1F1F | — | **$50.00** | — |
| 35 | `RAT-BLA` | Ratón de medias – Blanco | Juguetes | `raton-de-medias-blanco` | Blanco | #FFFFFF | — | **$50.00** | — |
| 36 | `RAT-GRI` | Ratón de medias – Gris | Juguetes | `raton-de-medias-gris` | Gris | #9E9E9E | — | **$50.00** | — |
| 37 | `RAT-VEB` | Ratón de medias – Verde y blanco | Juguetes | `raton-de-medias-verde-y-blanco` | Verde y blanco | #4CAF50 + #FFFFFF | — | **$50.00** | — |
| 38 | `RAT-AZB` | Ratón de medias – Azul con blanco | Juguetes | `raton-de-medias-azul-con-blanco` | Azul con blanco | #2F6FB5 + #FFFFFF | — | **$50.00** | — |
| 39 | `RAT-AZN` | Ratón de medias – Azul con negro | Juguetes | `raton-de-medias-azul-con-negro` | Azul con negro | #2F6FB5 + #1F1F1F | — | **$50.00** | — |
| 40 | `RAT-RSC` | Ratón de medias – Rosa con café | Juguetes | `raton-de-medias-rosa-con-cafe` | Rosa con café | #F4A6C1 + #7B5034 | — | **$50.00** | — |
| 41 | `RAT-NGR` | Ratón de medias – Negro con rojo | Juguetes | `raton-de-medias-negro-con-rojo` | Negro con rojo | #1F1F1F + #D32F2F | — | **$50.00** | — |
| 42 | `RAT-ROJ` | Ratón de medias – Rojo | Juguetes | `raton-de-medias-rojo` | Rojo | #D32F2F | — | **$50.00** | — |
| 43 | `RAT-MRS` | Ratón de medias – Morado con rosa | Juguetes | `raton-de-medias-morado-con-rosa` | Morado con rosa | #8E5BB5 + #F4A6C1 | — | **$50.00** | — |
| 44 | `CUE-AZU` | Cuerda de juego – Azul | Juguetes | `cuerda-de-juego-azul` | Azul | #2F6FB5 | — | **$50.00** | — |
| 45 | `CUE-ROS` | Cuerda de juego – Rosa | Juguetes | `cuerda-de-juego-rosa` | Rosa | #F4A6C1 | — | **$50.00** | — |
| 46 | `CUE-VER` | Cuerda de juego – Verde | Juguetes | `cuerda-de-juego-verde` | Verde | #4CAF50 | — | **$50.00** | — |
| 47 | `CUE-AMA` | Cuerda de juego – Amarillo | Juguetes | `cuerda-de-juego-amarillo` | Amarillo | #F5D04C | — | **$50.00** | — |
| 48 | `CUE-MOR` | Cuerda de juego – Morado | Juguetes | `cuerda-de-juego-morado` | Morado | #8E5BB5 | — | **$50.00** | — |
| 49 | `CUE-NEG` | Cuerda de juego – Negro | Juguetes | `cuerda-de-juego-negro` | Negro | #1F1F1F | — | **$50.00** | — |
| 50 | `CUE-BLA` | Cuerda de juego – Blanco | Juguetes | `cuerda-de-juego-blanco` | Blanco | #FFFFFF | — | **$50.00** | — |
| 51 | `CUE-GRI` | Cuerda de juego – Gris | Juguetes | `cuerda-de-juego-gris` | Gris | #9E9E9E | — | **$50.00** | — |
| 52 | `CUE-VEB` | Cuerda de juego – Verde y blanco | Juguetes | `cuerda-de-juego-verde-y-blanco` | Verde y blanco | #4CAF50 + #FFFFFF | — | **$50.00** | — |
| 53 | `CUE-AZB` | Cuerda de juego – Azul con blanco | Juguetes | `cuerda-de-juego-azul-con-blanco` | Azul con blanco | #2F6FB5 + #FFFFFF | — | **$50.00** | — |
| 54 | `CUE-AZN` | Cuerda de juego – Azul con negro | Juguetes | `cuerda-de-juego-azul-con-negro` | Azul con negro | #2F6FB5 + #1F1F1F | — | **$50.00** | — |
| 55 | `CUE-RSC` | Cuerda de juego – Rosa con café | Juguetes | `cuerda-de-juego-rosa-con-cafe` | Rosa con café | #F4A6C1 + #7B5034 | — | **$50.00** | — |
| 56 | `CUE-NGR` | Cuerda de juego – Negro con rojo | Juguetes | `cuerda-de-juego-negro-con-rojo` | Negro con rojo | #1F1F1F + #D32F2F | — | **$50.00** | — |
| 57 | `CUE-ROJ` | Cuerda de juego – Rojo | Juguetes | `cuerda-de-juego-rojo` | Rojo | #D32F2F | — | **$50.00** | — |
| 58 | `CUE-MRS` | Cuerda de juego – Morado con rosa | Juguetes | `cuerda-de-juego-morado-con-rosa` | Morado con rosa | #8E5BB5 + #F4A6C1 | — | **$50.00** | — |
| 59 | `COJ-AZU` | Cojín rellenable – Azul | Cojines | `cojin-rellenable-azul` | Azul | #2F6FB5 | ~~$129.00~~ | **$50.00** | -61% |
| 60 | `COJ-ROS` | Cojín rellenable – Rosa | Cojines | `cojin-rellenable-rosa` | Rosa | #F4A6C1 | ~~$129.00~~ | **$50.00** | -61% |
| 61 | `COJ-VER` | Cojín rellenable – Verde | Cojines | `cojin-rellenable-verde` | Verde | #4CAF50 | ~~$129.00~~ | **$50.00** | -61% |
| 62 | `COJ-AMA` | Cojín rellenable – Amarillo | Cojines | `cojin-rellenable-amarillo` | Amarillo | #F5D04C | ~~$129.00~~ | **$50.00** | -61% |
| 63 | `COJ-MOR` | Cojín rellenable – Morado | Cojines | `cojin-rellenable-morado` | Morado | #8E5BB5 | ~~$129.00~~ | **$50.00** | -61% |
| 64 | `COJ-NEG` | Cojín rellenable – Negro | Cojines | `cojin-rellenable-negro` | Negro | #1F1F1F | ~~$129.00~~ | **$50.00** | -61% |
| 65 | `COJ-BLA` | Cojín rellenable – Blanco | Cojines | `cojin-rellenable-blanco` | Blanco | #FFFFFF | ~~$129.00~~ | **$50.00** | -61% |
| 66 | `COJ-GRI` | Cojín rellenable – Gris | Cojines | `cojin-rellenable-gris` | Gris | #9E9E9E | ~~$129.00~~ | **$50.00** | -61% |
| 67 | `COJ-VEB` | Cojín rellenable – Verde y blanco | Cojines | `cojin-rellenable-verde-y-blanco` | Verde y blanco | #4CAF50 + #FFFFFF | ~~$129.00~~ | **$50.00** | -61% |
| 68 | `COJ-AZB` | Cojín rellenable – Azul con blanco | Cojines | `cojin-rellenable-azul-con-blanco` | Azul con blanco | #2F6FB5 + #FFFFFF | ~~$129.00~~ | **$50.00** | -61% |
| 69 | `COJ-AZN` | Cojín rellenable – Azul con negro | Cojines | `cojin-rellenable-azul-con-negro` | Azul con negro | #2F6FB5 + #1F1F1F | ~~$129.00~~ | **$50.00** | -61% |
| 70 | `COJ-RSC` | Cojín rellenable – Rosa con café | Cojines | `cojin-rellenable-rosa-con-cafe` | Rosa con café | #F4A6C1 + #7B5034 | ~~$129.00~~ | **$50.00** | -61% |
| 71 | `COJ-NGR` | Cojín rellenable – Negro con rojo | Cojines | `cojin-rellenable-negro-con-rojo` | Negro con rojo | #1F1F1F + #D32F2F | ~~$129.00~~ | **$50.00** | -61% |
| 72 | `COJ-ROJ` | Cojín rellenable – Rojo | Cojines | `cojin-rellenable-rojo` | Rojo | #D32F2F | ~~$129.00~~ | **$50.00** | -61% |
| 73 | `COJ-MRS` | Cojín rellenable – Morado con rosa | Cojines | `cojin-rellenable-morado-con-rosa` | Morado con rosa | #8E5BB5 + #F4A6C1 | ~~$129.00~~ | **$50.00** | -61% |

> El mismo catálogo se adjunta como `catalogo_semilla.json` y `catalogo_semilla.csv` (con columnas `precio_regular_mxn`, `precio_final_mxn`, `en_oferta`, `descuento_pct`) para generar `supabase/seed.sql`.

---

## 9. Reglas comerciales de envío

| Tipo | Cobertura | Costo | Paquetería | Tiempo de entrega |
|---|---|---|---|---|
| **Entrega local** | Reynosa, Tamaulipas (zona metropolitana) | **Gratis ($0.00)** | Reparto local LOKY PET | 24 a 48 horas hábiles |
| **Envío nacional** | Cualquier destino **fuera de Reynosa** en México | **$139.00 MXN por pedido** | **DHL** | 3 a 5 días hábiles |

- Implementar en `ShippingPolicy` (Domain) con valores configurables (`appsettings.json`), no hardcodeados en la UI.
- Los pedidos se procesan en 24 horas posteriores a la confirmación del pago; envíos de lunes a viernes en días hábiles (literal de 10.3).
- Mostrar estas reglas en: barra de anuncios, detalle de producto, carrito, checkout, confirmación y en el bloque "Tarifas de envío" de la pestaña Envíos y Devoluciones (10.3).

---

## 10. Contenido de "Sobre nosotros"

> ⚠️ **REGLA ABSOLUTA PARA LOS BLOQUES [TEXTO LITERAL]:** el texto proviene de imágenes de referencia y debe aparecer **exactamente igual**: mismas palabras, mayúsculas, acentos, signos de puntuación, números (incluida la numeración "1.", "2.", "3.", "4." de los títulos), negritas y orden. **No resumir, no reescribir, no traducir, no "mejorar", no omitir.** Solo se adaptan los colores al tema `lokypet`; la estructura (columnas, tarjetas, acordeones, pestañas, íconos) debe respetar la referencia.
>
> Los bloques marcados **[TEXTO REDACTADO]** sí fueron escritos para este proyecto y pueden ajustarse si yo lo pido.

### 10.1 Footer corporativo — [TEXTO LITERAL] (fondo café oscuro, 3 columnas + barra inferior; se usa en todo el sitio)

**Columna 1** — ícono `fa-paw` en cuadro redondeado + título:
- **LOKY PET, S.A. DE C.V.**
- Empresa mexicana dedicada a la confección sustentable de productos para mascotas mediante el reciclaje y reutilización creativa de textiles en Reynosa, Tamaulipas.
- `RFC: LPE260115H30` *(fuente monoespaciada)*

**Columna 2** — título: **CONTACTO CORPORATIVO**
- (ícono ubicación) Blvd. del Maestro No. 320, Local 4-B, Fracc. Las Fuentes, C.P. 88710, Reynosa, Tamaulipas.
- (ícono sobre) contacto@lokypet.com.mx
- (ícono teléfono) +52 (899) 900-LOKY (5659)

**Columna 3** — título: **CUMPLIMIENTO LEGAL**
- Empresa constituida formalmente con registro ante el SAT, IMPI (Clase 18), IMSS y en cumplimiento de las NOM de la STPS para talleres textiles.
- Recuadro con borde e ícono de carpeta abierta: Los documentos comprobatorios de cumplimiento legal se encuentran ordenados en el apartado de **Anexos** del expediente corporativo.

**Barra inferior** (separada por una línea):
- Izquierda: © 2026 LOKY PET, S.A. DE C.V. Todos los derechos reservados.
- Derecha (enlaces): Términos · Privacidad · Sustentabilidad → abren `/sobre-nosotros?tab=terminos`, `?tab=privacidad` y `?tab=sustentabilidad`.

### 10.2 Encabezado del Centro de Transparencia Legal — [TEXTO LITERAL]

**Barra de marca:**
- Logotipo (pata) + **LOKY PET** y debajo **S.A. DE C.V. • CIRCULAR ECONOMY**
- Insignias a la derecha: (ícono ubicación) **Reynosa, Tamaulipas** · (ícono escudo) **Legal & Compliance**

**Hero:**
- Etiqueta tipo píldora: **CENTRO DE TRANSPARENCIA LEGAL**
- Título: **Términos, Condiciones y Políticas Operativas**
- Texto: Conoce las bases legales, ambientales y comerciales que rigen nuestras operaciones de **upcycling textil** para mascotas. En **LOKY PET, S.A. DE C.V.** priorizamos la transparencia, la seguridad del consumidor y el impacto ecológico positivo.
- (ícono calendario) Última actualización: Octubre 2026 | Aplicable en México

**Pestañas** (desplazables horizontalmente en móvil, con ícono; la activa en café con texto crema):
1. (ícono documento) **Términos y Condiciones** — activa por defecto
2. (ícono usuario con escudo) **Aviso de Privacidad**
3. (ícono camión) **Envíos y Devoluciones**
4. (ícono hoja) **Garantía de Sustentabilidad**

### 10.3 Contenido asignado a cada pestaña

| Pestaña | Contenido (en este orden) |
|---|---|
| **Términos y Condiciones** | Acordeón **4. Propiedad Intelectual** [TEXTO LITERAL] |
| **Aviso de Privacidad** | Aviso de Privacidad (10.4) [TEXTO REDACTADO] |
| **Envíos y Devoluciones** | Encabezado + 3 tarjetas + acordeones **1**, **2** y **3** [TEXTO LITERAL] + bloque "Tarifas de envío" [TEXTO REDACTADO] |
| **Garantía de Sustentabilidad** | Garantía de Sustentabilidad (10.5) [TEXTO REDACTADO] |

Cambio de pestaña con transición *fade/slide* de 250 ms; acordeones abiertos por defecto, con ícono a la izquierda y chevron a la derecha.

#### Pestaña "Términos y Condiciones" — [TEXTO LITERAL]

**4. Propiedad Intelectual** (ícono "R" de marca registrada)
> La marca mixta **LOKY PET**, logotipos, diseños de moldes, fotografías y contenidos del sitio web son propiedad exclusiva de LOKY PET, S.A. DE C.V., protegidos bajo la Ley Federal de Protección a la Propiedad Industrial e inscritos o en trámite ante el Instituto Mexicano de la Propiedad Industrial (IMPI) bajo la Clase 18.

#### Pestaña "Envíos y Devoluciones" — [TEXTO LITERAL]

Encabezado con ícono (carrito/camión con paquete):
- Título: **Política de Envíos, Entrega y Devoluciones**
- Subtítulo: Criterios de logística comercial y satisfacción del cliente

**Tres tarjetas** (ícono + título + texto):

| Ícono | Título | Texto |
|---|---|---|
| Edificios (`fa-city`) | **Entrega Local Reynosa** | 24 a 48 horas hábiles en zona metropolitana de Reynosa. |
| Mapa con ubicación (`fa-map-location-dot`) | **Envíos Nacionales** | 3 a 5 días hábiles mediante paqueterías autorizadas en México. |
| Flecha de retorno (`fa-rotate-left`) | **Garantía de Devolución** | Hasta 10 días naturales por defectos de confección o empaque. |

**1. Tiempos de Procesamiento y Despacho** (ícono caja abierta)
> Los pedidos se procesan y preparan en nuestra área de empaque en Reynosa en un plazo de 24 horas posteriores a la confirmación del pago. Los envíos se realizan de lunes a viernes en días hábiles.

**2. Condiciones para Cambios o Devoluciones** (ícono flechas circulares)
> Para hacer efectiva una devolución, el producto debe encontrarse sin uso, con sus etiquetas originales de tela y sin señales de desgaste ajenas al proceso de confección. No aplican devoluciones por daños causados por el uso indebido o mordeduras destructivas extremas de la mascota.

**3. Precios, Facturación y Pagos** (ícono tarjeta de crédito)
> Todos los precios expuestos están expresados en pesos mexicanos (MXN) e incluyen el Impuesto al Valor Agregado (IVA) correspondiente.
>
> Los pagos se procesan a través de pasarelas de pago seguras cifradas (Shopify Payments, Mercado Pago, tarjetas de crédito/débito y pagos en efectivo en tiendas de conveniencia). LOKY PET emite Comprobantes Fiscales Digitales por Internet (CFDI 4.0) previa solicitud del cliente en un lapso no mayor a 5 días hábiles posteriores a la compra.

**Bloque "Tarifas de envío"** — [TEXTO REDACTADO] (tarjeta destacada después del acordeón 3, con ícono de camión, separada visualmente del texto literal)
> **Tarifas de envío**
> - **Entrega local en Reynosa:** sin costo.
> - **Envíos fuera de Reynosa:** $139.00 MXN por pedido. Todos los envíos nacionales se realizan por medio de **DHL**.
>
> Al despachar tu pedido nacional recibirás tu número de guía DHL para darle seguimiento.

### 10.4 Pestaña "Aviso de Privacidad" — [TEXTO REDACTADO]

> Nota para la IA: mostrar con el mismo estilo de acordeones numerados de las referencias (todos abiertos por defecto). Texto de referencia elaborado con la información de la empresa; no agregar datos nuevos (correos, teléfonos, autoridades) que no estén aquí.

Encabezado (ícono usuario con escudo):
- Título: **Aviso de Privacidad Integral**
- Subtítulo: Protección de datos personales de nuestros clientes

**1. Identidad y Domicilio del Responsable** (ícono edificio)
> LOKY PET, S.A. DE C.V., con domicilio en Blvd. del Maestro No. 320, Local 4-B, Fracc. Las Fuentes, C.P. 88710, Reynosa, Tamaulipas, es responsable del tratamiento de tus datos personales conforme a la Ley Federal de Protección de Datos Personales en Posesión de los Particulares.

**2. Datos Personales que Recabamos** (ícono lista)
> Para atender tus compras y solicitudes recabamos datos de identificación y contacto (nombre, correo electrónico y teléfono), datos de entrega (domicilio y código postal), datos fiscales cuando solicitas factura (RFC, razón social, régimen fiscal y código postal fiscal), tu historial de pedidos y los mensajes que nos envías mediante el formulario de contacto. LOKY PET no recaba datos personales sensibles. Los datos de tarjetas bancarias son procesados directamente por las pasarelas de pago y no se almacenan en nuestros sistemas.

**3. Finalidades del Tratamiento** (ícono diana)
> **Finalidades primarias:** procesar y entregar tus pedidos, gestionar envíos locales y nacionales, emitir tu Comprobante Fiscal Digital por Internet (CFDI 4.0), atender cambios, devoluciones y garantías, administrar tu cuenta y responder tus preguntas.
>
> **Finalidades secundarias:** enviarte novedades, ofertas y encuestas de satisfacción. Puedes negarte a estas finalidades en cualquier momento escribiendo a contacto@lokypet.com.mx, sin que ello afecte tus compras.

**4. Transferencia de Datos** (ícono flechas de intercambio)
> Únicamente compartimos los datos necesarios con: la paquetería DHL, para la entrega de envíos nacionales; las pasarelas de pago, para procesar tus compras; y el Servicio de Administración Tributaria (SAT), para la emisión de facturas. No vendemos ni rentamos tus datos personales a terceros.

**5. Derechos ARCO y Revocación del Consentimiento** (ícono usuario con engrane)
> Tienes derecho a Acceder, Rectificar y Cancelar tus datos personales, así como a Oponerte a su tratamiento (derechos ARCO) o revocar tu consentimiento. Envía tu solicitud a contacto@lokypet.com.mx indicando tu nombre, el derecho que deseas ejercer y una identificación oficial. Daremos respuesta en un plazo máximo de 20 días hábiles.

**6. Uso de Tecnologías de Almacenamiento Local** (ícono galleta)
> Nuestro sitio utiliza el almacenamiento local de tu navegador para conservar tu carrito de compras y tu sesión. Esta información permanece en tu dispositivo y puedes eliminarla desde la configuración de tu navegador.

**7. Cambios al Aviso de Privacidad** (ícono campana)
> Cualquier modificación a este aviso será publicada en esta sección del sitio web de LOKY PET, indicando la fecha de su última actualización.

### 10.5 Pestaña "Garantía de Sustentabilidad" — [TEXTO REDACTADO]

> Nota para la IA: mismo estilo visual (tarjetas destacadas + acordeones numerados). No agregar cifras, certificaciones ni porcentajes que no estén aquí.

Encabezado (ícono hoja):
- Título: **Garantía de Sustentabilidad**
- Subtítulo: Nuestro compromiso con la economía circular y el upcycling textil

**Tres tarjetas:**

| Ícono | Título | Texto |
|---|---|---|
| `fa-recycle` | **Textiles con Segunda Vida** | Camisetas, medias, mezclilla y retazos de algodón transformados en productos para mascotas. |
| `fa-scissors` | **Confección Local** | Cada pieza se elabora en nuestro taller de Reynosa, Tamaulipas. |
| `fa-shield-heart` | **Calidad Respaldada** | Hasta 10 días naturales de garantía por defectos de confección o empaque. |

**1. Origen de Nuestros Materiales** (ícono hoja)
> Nuestros productos se elaboran a partir de textiles reutilizados: tiras de camisetas, medias, mezclilla, algodón y retazos de tela que se seleccionan, limpian y clasifican antes de su confección. Al reutilizarlos, evitamos que terminen como residuo y reducimos la necesidad de producir materiales nuevos.

**2. Proceso de Upcycling** (ícono tijeras)
> Cada textil se corta y se confecciona de forma artesanal aprovechando al máximo el material: los sobrantes del corte se utilizan como relleno para cojines y juguetes. Por la naturaleza del upcycling, cada pieza puede presentar ligeras variaciones de tono o textura que la hacen única.

**3. Bienestar y Seguridad de tu Mascota** (ícono pata)
> Diseñamos cada producto pensando en la comodidad de tu mascota, con costuras resistentes y materiales suaves. Recomendamos supervisar a tu mascota durante el juego y revisar periódicamente el estado del producto.

**4. Durabilidad y Cuidado** (ícono gota)
> Prolonga la vida de tu producto lavándolo a mano o en ciclo delicado con agua fría y dejándolo secar a la sombra. Un producto que dura más es un producto más sustentable.

**5. Empaque Responsable** (ícono caja)
> Procuramos utilizar empaques reducidos y reutilizables, priorizando materiales reciclables en nuestros envíos.

**6. Nuestro Compromiso** (ícono apretón de manos)
> Si tu producto presenta un defecto de confección o empaque, lo atendemos dentro de los 10 días naturales posteriores a la entrega conforme a nuestra Política de Envíos, Entrega y Devoluciones. Así garantizamos que cada pieza cumpla con la calidad que tu mascota merece y con nuestro compromiso con el planeta.

### 10.6 Historia, Misión, Visión y Valores — [TEXTO REDACTADO]

> Diseño atractivo: bloques alternos imagen/texto para la historia, tarjetas con ícono para misión y visión, y grid de 6 tarjetas para valores, con separadores de ondas suaves.

**Nuestra historia**
LOKY PET nace en Reynosa, Tamaulipas, con una idea sencilla: las telas que ya cumplieron su ciclo pueden convertirse en algo que haga feliz a una mascota. Camisetas, medias y mezclilla que antes terminaban en la basura hoy se transforman, en nuestro taller, en camas, juguetes y cojines hechos a mano, resistentes y llenos de color.

**Misión**
Confeccionar productos para mascotas funcionales, seguros y con estilo a partir de textiles reciclados, ofreciendo a las familias de México una opción accesible que cuida a sus mascotas y al planeta al mismo tiempo.

**Visión**
Ser la marca mexicana referente en accesorios para mascotas de economía circular, reconocida por la calidad de su confección, la transparencia de sus operaciones y su impacto ambiental positivo.

**Valores**
- **Sustentabilidad:** cada producto le da una segunda vida a un textil.
- **Bienestar animal:** diseñamos pensando en la comodidad y seguridad de cada mascota.
- **Transparencia:** operamos con reglas claras y cumplimiento legal.
- **Creatividad:** convertimos retazos en piezas únicas y divertidas.
- **Calidad artesanal:** confección cuidadosa, pieza por pieza.
- **Compromiso local:** orgullosamente hechos en Reynosa, Tamaulipas.

No mostrar cifras de impacto (kg reciclados, piezas vendidas, etc.) salvo que yo proporcione datos reales.

---

## 11. Base de datos (Supabase) — esquema propuesto

Tablas (snake_case, PK `uuid`, `created_at`/`updated_at`):

- `categories` (id, name, slug, sort_order) → Camas, Juguetes, Cojines.
- `products` (id, category_id, name, slug, description, is_active)
- `product_variants` (id, product_id, sku único, slug único, color_name, color_hex text[], **price_mxn** numeric(10,2) [precio final], **compare_at_price_mxn** numeric(10,2) null [precio anterior; si tiene valor > price_mxn, el ítem está en oferta], stock int ≥ 0, is_out_of_stock, is_active)
- `product_images` (id, product_id, variant_id null, storage_path, sort_order, alt_text)
- `user_profiles` (id = auth.users.id, full_name, phone, default_address jsonb, avatar_key, role ['customer','admin'])
- `orders` (id, order_number, user_id null, customer jsonb, shipping_address jsonb, shipping_type ['local','nacional'], shipping_carrier null|'DHL', tracking_number null, subtotal, discount_total, shipping_cost [0 | 139], total, status, is_simulated = true)
- `order_items` (id, order_id, variant_id, sku, name, color, unit_price, compare_at_price, quantity, line_total)
- `inventory_movements` (id, variant_id, delta, reason, user_id)
- `contact_messages` (id, name, email, phone, subject, message, status ['nuevo','leido','respondido'])

❌ No crear tablas ni columnas de tallas o medidas.

**RLS:**
- Lectura pública del catálogo solo si `is_active = true`.
- Escritura de catálogo/inventario, gestión de pedidos y lectura de mensajes: solo `role = 'admin'`.
- `orders`/`order_items`: cada cliente lee solo los suyos; la creación del pedido, el cálculo del envío y el descuento de stock se hacen en una **función RPC transaccional** (`create_simulated_order`) que valida stock y recalcula precios y envío en el servidor (no confiar en los totales del cliente).
- `contact_messages`: `insert` público; `select/update` solo admin.
- Storage: bucket `product-images` con lectura pública y escritura solo admin.

---

## 12. Requisitos no funcionales

- **Responsive** mobile-first (360 px → 1920 px).
- **Rendimiento**: imágenes WebP con `loading="lazy"`, skeletons, compresión Brotli, *lazy loading* de ensamblados del dashboard si es viable.
- **Accesibilidad**: `alt` descriptivos, foco visible, navegación por teclado, `aria-*` en acordeones, pestañas, carruseles y drawers.
- **SEO básico**: `PageTitle` y `HeadContent` por página.
- **Formato**: moneda `es-MX` (`$1,234.00`), fechas en español.
- **Errores**: toasts amigables y página 404 con temática de mascotas.
- **Pruebas** unitarias (Domain/Application): totales del carrito, ahorro por ofertas, % de descuento, regla de envío (Reynosa $0 / fuera $139 DHL), validación de stock.

---

## 13. Despliegue en Vercel

- `dotnet publish src/LokyPet.Client -c Release -o output` → servir `output/wwwroot`.
- Vercel no incluye .NET: instalar el SDK en el *build command* con `dotnet-install.sh`, **o** compilar en GitHub Actions y desplegar con `vercel deploy --prebuilt`. Propón la opción y explícame los pasos.
- Compilar Tailwind/DaisyUI antes del `dotnet publish`.
- `vercel.json` con *rewrite* de todas las rutas a `/index.html` (p. ej. `/accesorios/cama-azul` debe funcionar al recargar) y tipos MIME correctos para `.wasm`/`.dat`.
- Variables de entorno: `SUPABASE_URL`, `SUPABASE_ANON_KEY` (inyectadas en `appsettings.json` durante el build).
- `README.md` con instalación local, configuración de Supabase (migraciones + seed) y despliegue.

---

## 14. Plan de trabajo por fases (detente al final de cada una)

1. **Base y diseño:** solución Clean Architecture, Tailwind + DaisyUI con tema `lokypet`, componentes base, layouts, navbar animada, barra de anuncios, footer, rutas vacías → **presentar el sistema de diseño para aprobación (4.7)**.
2. **Supabase:** migraciones, RLS, Storage, `seed.sql` con los 73 ítems y sus ofertas, repositorios.
3. **Catálogo:** Home, Accesorios (filtros/orden), detalle de producto, similares.
4. **Carrito y pago simulado:** carrito, drawer, checkout por pasos con regla de envío, confirmación, RPC de pedido.
5. **Sobre nosotros y Contacto:** contenido de la sección 10 (literal + redactado), pestañas con parámetro de URL, formulario, redes sociales.
6. **Mi Cuenta:** auth, perfil, avatares, historial de pedidos.
7. **Dashboard:** resumen con gráficas, inventario/productos/ofertas, pedidos, mensajes.
8. **Pulido y despliegue:** revisión visual completa, microinteracciones, accesibilidad, rendimiento, pruebas, Vercel, README.

---

## 15. Decisiones

### 15.1 Ya tomadas (no volver a preguntar)
- **Ofertas**: Cama $199 → **$160**; Cojín rellenable $129 → **$50**. Juguetes sin oferta ($50).
- **Tallas y medidas**: no se usan.
- **Categorías**: solo Camas, Juguetes y Cojines (sin trajes, collares ni alimento).
- **Envío**: gratis en Reynosa; **$139.00 MXN por pedido fuera de Reynosa, por DHL**.
- **Redes sociales**: solo Instagram (**@lokypet_**) y Facebook (**Loky Pet**), con enlaces temporales al inicio de sesión de cada plataforma. Sin WhatsApp.
- **Pestañas legales**: acordeón 3 → Envíos y Devoluciones; acordeón 4 → Términos y Condiciones. Aviso de Privacidad y Garantía de Sustentabilidad con texto redactado (10.4 y 10.5).
- **Checkout**: se permite compra como invitado.

### 15.2 Pendientes (pregúntame cuando llegues a ellas)
1. URLs definitivas de Instagram y Facebook (solo se reemplazan en `appsettings.json`).
2. Stock inicial real por ítem (por defecto 10).
3. Fuentes tipográficas exactas y colores derivados del tema (propuesta en Fase 1).
4. Librería de gráficas del dashboard (propuesta en Fase 7).
5. Opción de despliegue en Vercel (build con .NET en Vercel o GitHub Actions).
