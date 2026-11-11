import './app.js';
import { apiFetch, getAssetUrl } from './api.js';
document.addEventListener('DOMContentLoaded', () => {
    void loadMyCourses();
});
async function loadMyCourses() {
    const container = document.getElementById('my-course-list');
    if (!container) {
        return;
    }
    if (!localStorage.getItem('token')) {
        window.location.href = 'login.html';
        return;
    }
    try {
        const courses = await apiFetch('/Enrollments/my-courses');
        container.innerHTML = '';
        if (courses.length === 0) {
            container.textContent = 'Henüz hiçbir kursa kayıt olmadınız.';
            return;
        }
        courses.forEach((course) => container.appendChild(createCourseCard(course)));
    }
    catch (error) {
        console.error(error);
        container.textContent = 'Kurslar yüklenirken bir hata oluştu.';
    }
}
function createCourseCard(course) {
    const card = document.createElement('div');
    card.className = 'course-card';
    const image = document.createElement('img');
    image.src = getAssetUrl(course.imageUrl) || 'https://via.placeholder.com/400x200';
    image.alt = course.title;
    image.className = 'course-image';
    card.append(image);
    const content = document.createElement('div');
    content.className = 'course-content';
    content.innerHTML = `
        <div class="course-category"></div>
        <h3 class="course-title"></h3>
        <div class="course-instructor"><span></span></div>
        <div class="course-footer"><a class="view-btn">Kursa Git</a></div>
    `;
    content.querySelector('.course-category').textContent = course.categoryName;
    content.querySelector('.course-title').textContent = course.title;
    content.querySelector('.course-instructor span').textContent = course.instructorName;
    content.querySelector('.view-btn').href =
        `course-details.html?id=${course.id}`;
    card.append(content);
    return card;
}
