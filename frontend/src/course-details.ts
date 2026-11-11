import {
    checkEnrollment,
    enroll,
    getAssetUrl,
    getCourse,
    getLessons,
} from './api.js';
import type { Course, Lesson } from './types.js';

const courseId = Number(new URLSearchParams(window.location.search).get('id'));
let currentCourse: Course | null = null;

document.addEventListener('DOMContentLoaded', () => {
    void loadCourseDetails();
});

async function loadCourseDetails(): Promise<void> {
    if (!Number.isInteger(courseId) || courseId <= 0) {
        alert('Geçersiz kurs ID');
        window.location.href = 'index.html';
        return;
    }

    try {
        currentCourse = await getCourse(courseId);
        document.getElementById('c-title')!.textContent = currentCourse.title;
        document.getElementById('c-desc')!.textContent = currentCourse.description;
        document.getElementById('c-category')!.textContent = currentCourse.categoryName;
        document.getElementById('c-instructor')!.textContent = `Eğitmen: ${currentCourse.instructorName}`;
        document.getElementById('c-price')!.textContent = `${currentCourse.price} TL`;
        const image = document.getElementById('c-image') as HTMLImageElement;
        image.src = getAssetUrl(currentCourse.imageUrl);
        image.alt = currentCourse.title;
        document.title = `${currentCourse.title} - EduCore`;
        await updateEnrollmentActions();
    } catch (error) {
        console.error(error);
        document.getElementById('c-title')!.textContent = 'Kurs yüklenemedi';
    }
}

async function updateEnrollmentActions(): Promise<void> {
    const buyButton = document.getElementById('btn-buy') as HTMLButtonElement;
    const goButton = document.getElementById('btn-go') as HTMLButtonElement;
    if (!localStorage.getItem('token')) {
        buyButton.style.display = 'inline-block';
        buyButton.onclick = () => { window.location.href = 'login.html'; };
        return;
    }

    try {
        if (await checkEnrollment(courseId)) {
            goButton.style.display = 'inline-block';
            await loadLessons();
        } else {
            buyButton.style.display = 'inline-block';
            buyButton.onclick = () => { void enrollCourse(); };
        }
    } catch (error) {
        console.error(error);
        buyButton.style.display = 'inline-block';
    }
}

async function enrollCourse(): Promise<void> {
    if (!localStorage.getItem('token')) {
        window.location.href = 'login.html';
        return;
    }
    if (!confirm('Bu kursu satın almak istediğinize emin misiniz?')) {
        return;
    }

    try {
        await enroll(courseId);
        alert('Kurs başarıyla hesabınıza tanımlandı. İyi dersler!');
        window.location.reload();
    } catch (error) {
        alert(error instanceof Error ? error.message : 'Kayıt işlemi başarısız oldu.');
    }
}

async function loadLessons(): Promise<void> {
    const container = document.getElementById('lessons-container')!;
    const lessons = await getLessons(courseId);
    container.innerHTML = '';
    if (lessons.length === 0) {
        container.textContent = 'Bu kursta henüz ders bulunmuyor.';
        return;
    }
    lessons.forEach((lesson, index) => {
        const item = document.createElement('div');
        item.className = 'lesson-item';
        item.innerHTML = `<span>${index + 1}. </span><span style="font-size:0.8rem; color:#aaa">Video</span>`;
        item.firstElementChild!.textContent += lesson.title;
        item.addEventListener('click', () => playVideo(lesson, item));
        container.appendChild(item);
    });
}

function playVideo(lesson: Lesson, element: HTMLElement): void {
    document.querySelectorAll('.lesson-item').forEach((item) => item.classList.remove('active'));
    element.classList.add('active');
    const player = document.getElementById('player-container')!;
    const video = document.getElementById('main-video') as HTMLVideoElement;
    player.style.display = 'block';
    video.src = getAssetUrl(lesson.videoUrl);
    document.getElementById('video-title')!.textContent = lesson.title;
    void video.play();
}

