import { login, register } from './api.js';
import type { AuthResponse } from './types.js';

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('login-form')?.addEventListener('submit', (event) => {
        void handleLogin(event);
    });
    document.getElementById('register-form')?.addEventListener('submit', (event) => {
        void handleRegister(event);
    });
});

function setMessage(message: string): void {
    const element = document.getElementById('error-message');
    if (element) {
        element.textContent = message;
    }
}

async function handleLogin(event: Event): Promise<void> {
    event.preventDefault();
    const form = event.currentTarget as HTMLFormElement;
    const data = new FormData(form);
    setMessage('Giriş yapılıyor...');

    try {
        const response = await login(
            String(data.get('username') ?? ''),
            String(data.get('password') ?? ''),
        );
        saveSession(response);
    } catch (error) {
        setMessage(error instanceof Error ? error.message : 'Giriş başarısız.');
    }
}

async function handleRegister(event: Event): Promise<void> {
    event.preventDefault();
    const form = event.currentTarget as HTMLFormElement;
    setMessage('Kayıt olunuyor...');

    try {
        const response = await register(new FormData(form));
        saveSession(response);
    } catch (error) {
        setMessage(error instanceof Error ? error.message : 'Kayıt başarısız.');
    }
}

function saveSession(data: AuthResponse): void {
    if (data.role === 'Instructor' && !data.token) {
        alert('Kaydınız başarıyla alındı. Yönetici onayı bekleniyor. Onaylandığında giriş yapabilirsiniz.');
        window.location.href = 'login.html';
        return;
    }

    localStorage.setItem('token', data.token);
    localStorage.setItem('username', data.username);
    localStorage.setItem('role', data.role);

    window.location.href = data.role === 'SuperAdmin'
        ? 'admin-dashboard.html'
        : data.role === 'Instructor'
            ? 'instructor-dashboard.html'
            : 'index.html';
}
