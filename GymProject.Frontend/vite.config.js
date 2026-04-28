import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  server: {
    host: '0.0.0.0',
    proxy: {
      '/api': {
        target: 'http://localhost:5134',
        changeOrigin: true,
        rewrite: (path) => path
      }
    }
  },
  preview: {
    host: '0.0.0.0'
  },
  plugins: [react()]
});
