/**
 * UserStore: Manejador de estado en el cliente (Estilo Zustand Persistence)
 * Persiste los datos básicos del usuario en LocalStorage.
 */
const UserStore = {
    key: 'upscale_app_user',

    /**
     * Guarda los datos del usuario en LocalStorage
     * @param {Object} userData { nombre, rol, id }
     */
    save: (userData) => {
        if (!userData) return;
        localStorage.setItem(UserStore.key, JSON.stringify(userData));
        console.log('UserStore: Sesión persistida en el cliente.');
    },

    /**
     * Obtiene los datos del usuario guardados
     * @returns {Object|null}
     */
    get: () => {
        const data = localStorage.getItem(UserStore.key);
        return data ? JSON.parse(data) : null;
    },

    /**
     * Elimina todos los datos del usuario (Cierre de sesión)
     */
    clear: () => {
        localStorage.removeItem(UserStore.key);
        console.log('UserStore: Sesión eliminada del cliente.');
    },

    /**
     * Verifica si hay una sesión activa en el cliente
     */
    hasSession: () => !!localStorage.getItem(UserStore.key)
};
