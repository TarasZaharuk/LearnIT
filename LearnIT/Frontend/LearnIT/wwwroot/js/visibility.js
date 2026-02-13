let visibilityHandler;

window.registerVisibilityChangeHandler = (dotNetObjRef) => {

    visibilityHandler = () => {
        dotNetObjRef.invokeMethodAsync(
            'OnTabVisibilityChanged',
            document.visibilityState
        );
    };

    document.addEventListener('visibilitychange', visibilityHandler);
};

window.unregisterVisibilityChangeHandler = () => {
    if (visibilityHandler) {
        document.removeEventListener('visibilitychange', visibilityHandler);
    }
};

