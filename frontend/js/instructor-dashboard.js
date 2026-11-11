import { createCourse, createLesson, deleteCourse, deleteLesson, getInstructorCourses, getInstructorStats, getCourse, getLessons, toggleCourse, updateCourse, updateLesson, uploadFile, } from './api.js';
let editingCourseId = null;
let editingLessonId = null;
let activeLessonCourseId = 0;
document.addEventListener('DOMContentLoaded', () => {
    if (localStorage.getItem('role') !== 'Instructor' || !localStorage.getItem('token')) {
        window.location.href = 'login.html';
        return;
    }
    document.getElementById('create-course-form')?.addEventListener('submit', (event) => {
        event.preventDefault();
        void saveCourse();
    });
    document.getElementById('add-lesson-form')?.addEventListener('submit', (event) => {
        event.preventDefault();
        void saveLesson();
    });
    document.getElementById('create-course-btn')?.addEventListener('click', () => {
        editingCourseId = null;
        document.getElementById('modal').style.display = 'flex';
    });
    document.querySelector('#modal button[type="button"]')
        ?.addEventListener('click', closeModal);
    document.querySelector('#lesson-modal button')
        ?.addEventListener('click', () => {
        document.getElementById('lesson-modal').style.display = 'none';
    });
    document.getElementById('course-list')?.addEventListener('click', (event) => {
        const target = event.target.closest('button[data-action]');
        if (!target)
            return;
        const id = Number(target.dataset.id);
        if (target.dataset.action === 'lessons')
            void openLessonModal(id, target.dataset.title || '');
        if (target.dataset.action === 'edit')
            void openEditCourse(id);
        if (target.dataset.action === 'toggle')
            void toggleCourseStatus(id);
        if (target.dataset.action === 'delete')
            void removeCourse(id);
    });
    void loadDashboard();
});
async function loadDashboard() {
    try {
        const courses = await getInstructorCourses();
        renderCourses(courses);
        document.getElementById('active-courses-count').textContent = String(courses.length);
        const stats = await getInstructorStats();
        document.getElementById('total-earnings').textContent = `${stats.totalEarnings.toFixed(2)} TL`;
        document.getElementById('total-students').textContent = String(stats.totalStudents);
    }
    catch (error) {
        console.error(error);
        document.getElementById('course-list').textContent = 'Dashboard yükleme hatası.';
    }
}
function renderCourses(courses) {
    const list = document.getElementById('course-list');
    list.innerHTML = '';
    if (!courses.length) {
        list.textContent = 'Henüz kurs oluşturulmadı.';
        return;
    }
    courses.forEach((course) => {
        const item = document.createElement('div');
        item.className = 'course-item';
        item.innerHTML = '<div class="course-info"><h4></h4><div class="course-meta"></div></div><div class="course-actions"></div>';
        item.querySelector('h4').textContent = course.title;
        item.querySelector('.course-meta').textContent =
            `${course.categoryName} - ${course.price} TL - ${course.status}`;
        const actions = item.querySelector('.course-actions');
        actions.innerHTML = `
            <button class="view-btn" data-action="lessons" data-id="${course.id}" data-title="${course.title}">İçerik</button>
            <button class="view-btn" data-action="edit" data-id="${course.id}">Düzenle</button>
            <button class="view-btn" data-action="toggle" data-id="${course.id}">${course.isActive ? 'Pasif' : 'Aktif'}</button>
            <button class="view-btn" data-action="delete" data-id="${course.id}" style="background:#ef4444">Sil</button>
        `;
        list.appendChild(item);
    });
}
async function openEditCourse(id) {
    try {
        const course = await getCourse(id);
        editingCourseId = id;
        document.getElementById('c-title').value = course.title;
        document.getElementById('c-desc').value = course.description;
        document.getElementById('c-price').value = String(course.price);
        document.getElementById('c-quota').value = String(course.quota);
        document.getElementById('c-image-url').value = course.imageUrl;
        document.getElementById('modal').style.display = 'flex';
    }
    catch (error) {
        alert(error instanceof Error ? error.message : 'Kurs bilgileri alınamadı.');
    }
}
async function toggleCourseStatus(id) {
    try {
        await toggleCourse(id);
        await loadDashboard();
    }
    catch (error) {
        alert(error instanceof Error ? error.message : 'Kurs durumu değiştirilemedi.');
    }
}
async function removeCourse(id) {
    if (!confirm('Bu kursu silmek istediğinize emin misiniz?'))
        return;
    try {
        await deleteCourse(id);
        await loadDashboard();
    }
    catch (error) {
        alert(error instanceof Error ? error.message : 'Kurs silinemedi.');
    }
}
async function saveCourse() {
    const file = document.getElementById('c-image-file').files?.[0];
    let imageUrl = document.getElementById('c-image-url').value || null;
    try {
        if (file)
            imageUrl = (await uploadFile('image', file)).url;
        const course = {
            title: document.getElementById('c-title').value,
            description: document.getElementById('c-desc').value,
            price: Number(document.getElementById('c-price').value),
            quota: Number(document.getElementById('c-quota').value),
            categoryId: Number(document.getElementById('c-category').value),
            imageUrl,
        };
        if (editingCourseId)
            await updateCourse(editingCourseId, course);
        else
            await createCourse(course);
        closeModal();
        await loadDashboard();
    }
    catch (error) {
        alert(error instanceof Error ? error.message : 'Kurs kaydedilemedi.');
    }
}
async function openLessonModal(courseId, title) {
    activeLessonCourseId = courseId;
    document.getElementById('lesson-modal-title').textContent = `${title} - Ders Yönetimi`;
    document.getElementById('lesson-modal').style.display = 'flex';
    await loadLessons(courseId);
}
async function loadLessons(courseId) {
    const list = document.getElementById('lesson-list');
    try {
        const lessons = await getLessons(courseId);
        list.innerHTML = '';
        lessons.forEach((lesson) => {
            const item = document.createElement('div');
            item.className = 'user-item';
            item.innerHTML = '<span></span><button class="action-btn" style="background:#ef4444;color:#fff">Sil</button>';
            item.querySelector('span').textContent = lesson.title;
            item.querySelector('button').addEventListener('click', () => void removeLesson(lesson.id));
            list.appendChild(item);
        });
        if (!lessons.length)
            list.textContent = 'Henüz ders eklenmedi.';
    }
    catch (error) {
        console.error(error);
        list.textContent = 'Dersler yüklenemedi.';
    }
}
async function saveLesson() {
    const file = document.getElementById('l-video-file').files?.[0];
    let videoUrl = document.getElementById('l-video-url').value || null;
    try {
        if (file)
            videoUrl = (await uploadFile('video', file)).url;
        const lesson = {
            title: document.getElementById('l-title').value,
            videoUrl,
            courseId: activeLessonCourseId,
        };
        if (editingLessonId)
            await updateLesson(editingLessonId, lesson);
        else
            await createLesson(lesson);
        resetLessonForm();
        await loadLessons(activeLessonCourseId);
    }
    catch (error) {
        alert(error instanceof Error ? error.message : 'Ders kaydedilemedi.');
    }
}
async function removeLesson(id) {
    if (!confirm('Bu dersi silmek istediğinize emin misiniz?'))
        return;
    try {
        await deleteLesson(id);
        await loadLessons(activeLessonCourseId);
    }
    catch (error) {
        alert(error instanceof Error ? error.message : 'Ders silinemedi.');
    }
}
function closeModal() {
    document.getElementById('modal').style.display = 'none';
}
function resetLessonForm() {
    document.getElementById('add-lesson-form').reset();
    editingLessonId = null;
}
