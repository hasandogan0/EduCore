const API_BASE_URL = 'http://localhost:5239/api';

document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }

    const registerForm = document.getElementById('register-form');
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
    }
});

async function handleLogin(e) {
    e.preventDefault();
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const errorMsg = document.getElementById('error-message');

    errorMsg.textContent = 'Giriş yapılıyor...';

    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });

        if (!response.ok) {
            const errText = await response.text();
            // Try to parse json error if possible, otherwise uses text
            try {
                const errJson = JSON.parse(errText);
                throw new Error(errJson.title || errText);
            } catch {
                throw new Error(errText || 'Giriş başarısız');
            }
        }

        const data = await response.json();
        saveSession(data);

    } catch (error) {
        errorMsg.textContent = error.message;
    }
}

async function handleRegister(e) {
    e.preventDefault();
    const fullname = document.getElementById('fullname').value;
    const username = document.getElementById('username').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const role = document.getElementById('role').value;
    const profileImageInput = document.getElementById('profileImage');
    const errorMsg = document.getElementById('error-message');

    errorMsg.textContent = 'Kayıt olunuyor...';

    const formData = new FormData();
    formData.append('fullname', fullname);
    formData.append('username', username);
    formData.append('email', email);
    formData.append('password', password);
    formData.append('role', role);

    if (profileImageInput && profileImageInput.files.length > 0) {
        formData.append('profileImage', profileImageInput.files[0]);
    }

    try {
        const response = await fetch(`${API_BASE_URL}/auth/register`, {
            method: 'POST',
            body: formData
        });

        if (!response.ok) {
            const errText = await response.text();
            try {
                const errJson = JSON.parse(errText);
                // If specific validation errors exist
                /*
                if(errJson.errors) {
                   const messages = Object.values(errJson.errors).flat().join(', ');
                   throw new Error(messages);
                }
                */
                throw new Error(errJson.title || errText);
            } catch {
                throw new Error(errText || 'Kayıt başarısız');
            }
        }

        const data = await response.json();
        saveSession(data);

    } catch (error) {
        errorMsg.textContent = error.message;
    }
}

function saveSession(data) {
    // If Instructor and Token is empty, it means Pending Approval
    if (data.role === 'Instructor' && !data.token) {
        alert('Kaydınız başarıyla alındı. Yönetici onayı bekleniyor. Onaylandığında giriş yapabilirsiniz.');
        window.location.href = 'login.html';
        return;
    }

    localStorage.setItem('token', data.token);
    localStorage.setItem('username', data.username);
    localStorage.setItem('role', data.role);

    // Redirect based on role
    if (data.role === 'SuperAdmin') {
        window.location.href = 'admin-dashboard.html';
    } else if (data.role === 'Instructor') {
        window.location.href = 'instructor-dashboard.html';
    } else {
        window.location.href = 'index.html';
    }
}
