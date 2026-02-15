/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      fontFamily: {
        display: ['"Space Grotesk"', 'system-ui', 'sans-serif'],
        serif: ['"Newsreader"', 'serif'],
      },
      colors: {
        ink: '#0f1419',
        bone: '#f5f1e8',
        sand: '#d9d2c5',
        ocean: '#0f3d3e',
        ember: '#e4572e',
        moss: '#3a5a40',
      },
      boxShadow: {
        glow: '0 12px 35px rgba(15, 61, 62, 0.25)',
      },
      borderRadius: {
        '3xl': '1.75rem',
      },
    },
  },
  plugins: [],
}
