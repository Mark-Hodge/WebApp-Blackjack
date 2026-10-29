/// <reference types="vitest/config" />
import react from '@vitejs/plugin-react'
import { defineConfig, type Plugin } from 'vite'

const apiHealthUrl = 'http://localhost:5080/health'

function openBrowserWhenApiIsReady(): Plugin {
  return {
    name: 'open-browser-when-api-is-ready',
    apply: 'serve',
    configureServer(server) {
      server.httpServer?.once('listening', async () => {
        await waitForApi()
        server.openBrowser()
      })
    },
  }
}

async function waitForApi(timeoutMs = 60_000, intervalMs = 500) {
  const deadline = Date.now() + timeoutMs
  while (Date.now() < deadline) {
    try {
      const response = await fetch(apiHealthUrl)
      if (response.ok) return
    } catch {
    }
    await new Promise((resolve) => setTimeout(resolve, intervalMs))
  }
  console.warn(`API not ready after ${timeoutMs / 1000}s, opening the browser anyway.`)
}

export default defineConfig({
  plugins: [react(), openBrowserWhenApiIsReady()],
  server: {
    port: 5173,
    strictPort: true,
    open: false,
  },
  test: {
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
    include: ['src/**/*.test.{ts,tsx}'],
  },
})