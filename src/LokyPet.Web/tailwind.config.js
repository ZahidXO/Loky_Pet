/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{razor,html,cs}",
    "../LokyPet.Application/**/*.cs",
  ],
  theme: {
    extend: {
      fontFamily: {
        // Caligráfica (solo logotipo y 1-2 titulares decorativos)
        script: ["Pacifico", "cursive"],
        // Sans-serif limpia para todo lo demás
        sans: ["Poppins", "ui-sans-serif", "system-ui", "sans-serif"],
      },
      borderRadius: {
        "2xl": "1rem",
      },
      boxShadow: {
        // Sombras suaves con tinte café (prohibidas sombras duras/negras)
        soft: "0 8px 24px rgba(96,71,52,.08)",
        "soft-lg": "0 16px 40px rgba(96,71,52,.12)",
        "soft-sm": "0 4px 12px rgba(96,71,52,.06)",
      },
      transitionTimingFunction: {
        loky: "cubic-bezier(0.22, 1, 0.36, 1)",
      },
      keyframes: {
        shimmer: {
          "0%": { backgroundPosition: "-468px 0" },
          "100%": { backgroundPosition: "468px 0" },
        },
        "fade-in-up": {
          "0%": { opacity: "0", transform: "translateY(8px)" },
          "100%": { opacity: "1", transform: "translateY(0)" },
        },
        "bounce-soft": {
          "0%, 100%": { transform: "scale(1)" },
          "50%": { transform: "scale(1.25)" },
        },
        "announce-fade": {
          "0%, 100%": { opacity: "0" },
          "10%, 90%": { opacity: "1" },
        },
      },
      animation: {
        shimmer: "shimmer 1.4s linear infinite",
        "fade-in-up": "fade-in-up 0.35s cubic-bezier(0.22, 1, 0.36, 1)",
        "bounce-soft": "bounce-soft 0.4s cubic-bezier(0.22, 1, 0.36, 1)",
      },
    },
  },
  plugins: [require("daisyui")],
  daisyui: {
    themes: [
      {
        lokypet: {
          // ===== Paleta oficial "Neapolitan" (obligatoria) =====
          "primary": "#604734",            // café chocolate
          "primary-content": "#fcf7d9",    // crema sobre café
          "secondary": "#f6d8d6",          // rosa pastel
          "secondary-content": "#604734",  // café sobre rosa pastel
          "accent": "#f0c5d4",             // rosa fresa
          "accent-content": "#604734",     // café sobre rosa fresa
          "base-100": "#fcf7d9",           // crema vainilla (fondo general)
          // ===== Derivados armónicos (propuestos) =====
          "neutral": "#8a7560",            // café grisáceo suave
          "neutral-content": "#fcf7d9",
          "base-200": "#f7eecb",           // crema un tono más profundo
          "base-300": "#efe2b8",           // crema borde/divisores
          "base-content": "#604734",       // texto principal café
          "info": "#6aa9c9",               // azul suave
          "info-content": "#1b2e36",
          "success": "#6fb97f",            // verde suave (económico/ecológico)
          "success-content": "#143018",
          "warning": "#e6b655",            // ámbar
          "warning-content": "#3a2a08",
          "error": "#d98a8a",              // rojo rosado suave
          "error-content": "#3a1212",
          // ===== Variables de componentes =====
          "--rounded-box": "1rem",         // tarjetas y modales (rounded-2xl)
          "--rounded-btn": "9999px",       // botones rounded-full
          "--rounded-badge": "9999px",     // chips/insignias rounded-full
          "--animation-btn": "0.25s",
          "--animation-input": "0.2s",
          "--btn-text-case": "none",
          "--border-btn": "1.5px",
          "--tab-radius": "0.75rem",
        },
      },
    ],
    darkTheme: false,
    base: true,
    styled: true,
    utils: true,
    logs: false,
  },
};
