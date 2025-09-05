import "./preline/preline.min.js";

function autoInit() {
    if (window.HSStaticMethods && typeof window.HSStaticMethods.autoInit === 'function') {
        window.HSStaticMethods.autoInit();
    }
}

function debounce(func, timeout = 300) {
    let timer;
    return (...args) => {
        clearTimeout(timer);
        timer = setTimeout(() => func.apply(this, args), timeout);
    };
}

const debouncedAutoInit = debounce(autoInit, 200);

const observer = new MutationObserver(() => {
    debouncedAutoInit();
});

observer.observe(document.body, {
    subtree: true,
    childList: true,
})