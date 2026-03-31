/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{vue,js,ts,jsx,tsx}",
    ],
    darkMode: 'class',
    theme: {
        fontFamily: {
            sans: ['Open Sans']
        },
        extend: {
            margin: {
                'menu': '15.75rem',
                'menu-contracted': '4.5rem'
            },
            fontSize: {
                '2xs': '0.6rem'
            },
            height: {
                '15': '3.75rem',
                'novo-pedido-card': 'calc(100vh - 12rem)'
            },
            width: {
                '15': '3.75rem',
                '18': '4.5rem',
                'menu': '15.75rem',
                'menu-contract': '4.5rem',
                'content': 'calc(100% - 15.75rem)',
                'content-contracted': 'calc(100% - 4.5rem)',
            },
            borderRadius: {
                '30px': '30px',
                '50px': '50px',
                '100px': '100px'
            },
            colors: {
                'neutral-100': '#FDFDFD',
                'neutral-200': '#F8F8F8',
                'neutral-300': '#F7F7F7',
                'neutral-400': '#F1F1F1',
                'neutral-900': 'var(--text-color)',
                'primary': 'var(--primary-color)',
                'primary-light': "var(--primary-color-light)",
                'accent': 'var(--accent-color)'
            },
            boxShadow: {
                main: '0px 5px 20px rgba(0, 0, 0, 0.07)',
                inset: 'inset 0px 0px 5px rgba(0, 0, 0, 0.1)'
            },
            translate: {
                'menu': '15.75rem',
            }
        },
    },
    plugins: [],
};