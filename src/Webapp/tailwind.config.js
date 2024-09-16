/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./Pages/**/*.cshtml", "./Areas/**/*.cshtml"],
  theme: {
    extend: {
      boxShadow: {
        hard: "5px -3px 0 0 rgba(0,0,0,0.75)",
        focus: "0px 0px 0 2px rgba(0,0,0,0.75)",
      },
    },
  },
  plugins: [],
};
