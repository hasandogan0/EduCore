import {
    adminAction,
    getAdminCourses,
    getAdminPendingCourses,
    getAdminPendingInstructors,
    getAdminUsers,
} from './api.js';
import type { AdminCourse, AdminUser, Course } from './types.js';

document.addEventListener('DOMContentLoaded', () => {
    if (localStorage.getItem('role') !== 'SuperAdmin' || !localStorage.getItem('token')) {
        window.location.href = 'login.html';
        return;
    }
    void refresh();
});

async function refresh(): Promise<void> {
    await Promise.all([
        loadPendingInstructors(),
        loadPendingCourses(),
        loadAllCourses(),
        loadAllUsers(),
    ]);
}

async function loadPendingInstructors(): Promise<void> {
    try {
        renderPendingInstructors(await getAdminPendingInstructors());
    } catch (error) {
        showError('instructor-list', error);
    }
}

function renderPendingInstructors(users: AdminUser[]): void {
    const list = document.getElementById('instructor-list')!;
    list.innerHTML = '';
    if (!users.length) {
        list.textContent = 'Bekleyen onay yok.';
        return;
    }
    users.forEach((user) => {
        const item = document.createElement('div');
        item.className = 'user-item';
        item.innerHTML = `<div><strong></strong><br><span style="font-size:0.9rem; color:var(--text-muted)"></span></div><div style="display:flex;gap:0.5rem"><button class="action-btn btn-approve">Onayla</button><button class="action-btn" style="background:#ef4444;color:#fff">Sil</button></div>`;
        item.querySelector('strong')!.textContent = user.username;
        item.querySelector('span')!.textContent = user.email;
        item.querySelector('.btn-approve')!.addEventListener('click', () => void runAction(
            `/Admin/approve-instructor/${encodeURIComponent(user.username)}`, loadPendingInstructors,
        ));
        item.querySelectorAll('button')[1].addEventListener('click', () => void runAction(
            `/Admin/instructor/${encodeURIComponent(user.username)}`, loadPendingInstructors, 'DELETE',
        ));
        list.appendChild(item);
    });
}

async function loadPendingCourses(): Promise<void> {
    try {
        renderPendingCourses(await getAdminPendingCourses());
    } catch (error) {
        showError('course-list', error);
    }
}

function renderPendingCourses(courses: Course[]): void {
    const list = document.getElementById('course-list')!;
    list.innerHTML = '';
    if (!courses.length) {
        list.textContent = 'Bekleyen kurs onayı yok.';
        return;
    }
    courses.forEach((course) => {
        const item = document.createElement('div');
        item.className = 'user-item';
        item.innerHTML = `<div><strong></strong><br><span></span></div><div><button class="action-btn btn-approve">Onayla</button><button class="action-btn" style="background:#ef4444;color:#fff">Sil</button></div>`;
        item.querySelector('strong')!.textContent = course.title;
        item.querySelector('span')!.textContent = `${course.categoryName} - ${course.price} TL`;
        item.querySelector('.btn-approve')!.addEventListener('click', () => void runAction(
            `/Admin/approve-course/${course.id}`, loadPendingCourses,
        ));
        item.querySelectorAll('button')[1].addEventListener('click', () => void runAction(
            `/Admin/course/${course.id}`, loadPendingCourses, 'DELETE',
        ));
        list.appendChild(item);
    });
}

async function loadAllCourses(): Promise<void> {
    try {
        const courses = await getAdminCourses();
        renderAdminCourses(courses);
    } catch (error) {
        showError('all-courses-list', error);
    }
}

function renderAdminCourses(courses: AdminCourse[]): void {
    const list = document.getElementById('all-courses-list')!;
    list.innerHTML = '';
    courses.forEach((course) => {
        const item = document.createElement('div');
        item.className = 'user-item';
        item.innerHTML = '<div><strong></strong><br><span></span></div><button class="action-btn"></button>';
        item.querySelector('strong')!.textContent = `${course.title} - ${course.instructor}`;
        item.querySelector('span')!.textContent = `Kazanç: ${course.totalEarnings} TL`;
        const button = item.querySelector('button')!;
        button.textContent = course.isActive ? 'Pasif Yap' : 'Aktif Yap';
        button.addEventListener('click', () => void runAction(
            `/Admin/toggle-course/${course.id}`, loadAllCourses,
        ));
        list.appendChild(item);
    });
}

async function loadAllUsers(): Promise<void> {
    try {
        const users = await getAdminUsers();
        renderAdminUsers(users);
    } catch (error) {
        showError('all-users-list', error);
    }
}

function renderAdminUsers(users: AdminUser[]): void {
    const list = document.getElementById('all-users-list')!;
    list.innerHTML = '';
    users.forEach((user) => {
        const item = document.createElement('div');
        item.className = 'user-item';
        item.innerHTML = '<div><strong></strong><br><span></span></div><button class="action-btn"></button>';
        item.querySelector('strong')!.textContent = `${user.username} (${user.role})`;
        item.querySelector('span')!.textContent = user.email;
        const button = item.querySelector('button')!;
        button.textContent = user.isActive ? 'Pasif Yap' : 'Aktif Yap';
        button.addEventListener('click', () => void runAction(
            `/Admin/toggle-user/${encodeURIComponent(user.id)}`, loadAllUsers,
        ));
        list.appendChild(item);
    });
}

async function runAction(path: string, reload: () => Promise<void>, method: 'POST' | 'DELETE' = 'POST'): Promise<void> {
    if (!confirm('Bu işlemi gerçekleştirmek istediğinize emin misiniz?')) return;
    try {
        await adminAction(path, method);
        await reload();
    } catch (error) {
        alert(error instanceof Error ? error.message : 'İşlem başarısız oldu.');
    }
}

function showError(id: string, error: unknown): void {
    console.error(error);
    document.getElementById(id)!.textContent = 'Veri yüklenemedi.';
}
