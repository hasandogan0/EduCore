import { checkEnrollment, getAssetUrl, getCourse, getLessons, getProgress, updateProgress } from './api.js';
const courseId = Number(new URLSearchParams(window.location.search).get('courseId'));
let totalLessons = 0;
let currentProgress = 0;
document.addEventListener('DOMContentLoaded', () => { void init(); });
async function init() {
    if (!localStorage.getItem('token')) {
        window.location.href = 'login.html';
        return;
    }
    if (!Number.isInteger(courseId) || courseId <= 0) {
        alert('Kurs ID eksik');
        window.location.href = 'index.html';
        return;
    }
    document.getElementById('back-btn').href =
        `course-details.html?id=${courseId}`;
    try {
        if (!await checkEnrollment(courseId)) {
            alert('Bu kursa erişim yetkiniz bulunmuyor.');
            window.location.href = `course-details.html?id=${courseId}`;
            return;
        }
        currentProgress = await getProgress(courseId);
        updateProgressUi();
        const course = await getCourse(courseId);
        document.getElementById('course-title').textContent = course.title;
        const lessons = await getLessons(courseId);
        totalLessons = lessons.length;
        renderLessons(lessons);
        if (lessons.length > 0)
            playLesson(lessons[0]);
        document.getElementById('main-video').addEventListener('ended', () => { void onVideoEnded(); });
    }
    catch (error) {
        console.error(error);
        alert(error instanceof Error ? error.message : 'İçerik yüklenemedi.');
    }
}
function renderLessons(lessons) {
    const list = document.getElementById('lesson-list');
    list.innerHTML = '';
    lessons.forEach((lesson, index) => {
        const item = document.createElement('div');
        item.className = 'lesson-item';
        item.id = `lesson-${lesson.id}`;
        item.innerHTML = `<div style="background:rgba(255,255,255,0.1); width:24px; height:24px; border-radius:50%; display:flex; align-items:center; justify-content:center; font-size:0.8rem">${index + 1}</div><div style="font-size:0.9rem"></div>`;
        item.lastElementChild.textContent = lesson.title;
        item.addEventListener('click', () => playLesson(lesson));
        list.appendChild(item);
    });
}
function playLesson(lesson) {
    document.querySelectorAll('.lesson-item').forEach((item) => item.classList.remove('active'));
    document.getElementById(`lesson-${lesson.id}`)?.classList.add('active');
    document.getElementById('video-title').textContent = lesson.title;
    const video = document.getElementById('main-video');
    video.src = getAssetUrl(lesson.videoUrl);
    void video.play();
}
async function onVideoEnded() {
    if (totalLessons === 0 || currentProgress >= 100)
        return;
    currentProgress = Math.min(100, currentProgress + Math.ceil(100 / totalLessons));
    updateProgressUi();
    try {
        await updateProgress(courseId, currentProgress);
    }
    catch (error) {
        console.error(error);
    }
}
function updateProgressUi() {
    const progress = document.getElementById('progress-info');
    progress.textContent = `${currentProgress}% Tamamlandı`;
    progress.style.color = currentProgress === 100 ? '#4ade80' : '#aaa';
}
