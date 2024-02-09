setInterval(() => {
    for (let key in localStorage) {
        if (key.startsWith('exp_')) {
            const dtValue = localStorage.getItem(key);
            const dt = Date.parse(dtValue);
            if (dt < Date.now()) {
                localStorage.removeItem(key);
                localStorage.removeItem('data_' + key.substring(4));
            }
        }
        else if (key.startsWith('data_')) {
            const mainKey = key.substring(5);
            if (!localStorage.getItem('exp_' + mainKey))
                localStorage.removeItem(key);
        }
    }
}, 30*1000);