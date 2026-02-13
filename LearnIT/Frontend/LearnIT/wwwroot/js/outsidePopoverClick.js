let outsideClickHandler;

window.registerOutsideClickListener = (dotNetHelper) => {

    outsideClickHandler = (event) => {
        const popover = document.querySelector(".popover");

        if (popover && !popover.contains(event.target)) {
            dotNetHelper.invokeMethodAsync("ClosePopover");
        }
    };

    document.addEventListener("click", outsideClickHandler);
};

window.unregisterOutsideClickListener = () => {
    if (outsideClickHandler) {
        document.removeEventListener("click", outsideClickHandler);
    }
};

