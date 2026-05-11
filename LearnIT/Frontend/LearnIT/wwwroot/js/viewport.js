let resizeHandler;

window.registerViewportChangeCallback = (dotnetHelper) => {

    resizeHandler = () => {
        dotnetHelper.invokeMethodAsync('OnResize', window.innerWidth, window.innerHeight);
    };

    window.addEventListener('resize', resizeHandler);

    // initial call
    resizeHandler();
};

window.unregisterViewportChangeCallback = () => {
    if (resizeHandler) {
        window.removeEventListener('resize', resizeHandler);
    }
};

