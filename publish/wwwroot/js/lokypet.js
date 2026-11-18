// LOKY PET — interop JS mínimo
window.lokypet = {
    // Notifica a Blazor cuando el scroll pasa un umbral (para compactar el navbar)
    registerScroll: function (dotNetRef, threshold) {
        threshold = threshold || 20;
        let last = null;
        const handler = () => {
            const scrolled = window.scrollY > threshold;
            if (scrolled !== last) {
                last = scrolled;
                dotNetRef.invokeMethodAsync('OnScroll', scrolled);
            }
        };
        window.addEventListener('scroll', handler, { passive: true });
        handler();
    }
};
