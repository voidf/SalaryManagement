
export const baseUrl = 'http://127.0.0.1:19999/api/v2';
export const ossUrl = baseUrl;

export const jwt = {
    value: ""
};

export default function U(url) {
    return `${baseUrl}${url}`;
}
// ---------------------------------------

// export default baseUrl;
