import { getAssetUrl, getCourses, getInstructors } from './api.js';
import type { Course, Instructor } from './types.js';

document.addEventListener('DOMContentLoaded', () => {
    updateNav();
    void loadCourses();
    void loadHomeInstructors();
});

function getRequiredElement<T extends HTMLElement>(id: string): T {
    const element = document.getElementById(id);
    if (!element) {
        throw new Error(`Sayfa öğesi bulunamadı: ${id}`);
    }
    return element as T;
}

function updateNav(): void {
    const user = localStorage.getItem('username');
    const navList = document.querySelector<HTMLElement>('nav ul');
    const loginLink = navList?.querySelector<HTMLAnchorElement>('.view-btn');

    if (!navList || !loginLink) {
        return;
    }

    document.getElementById('logout-btn-nav')?.remove();
    document.getElementById('my-courses-nav')?.remove();

    if (!user) {
        loginLink.textContent = 'Giriş Yap';
        loginLink.href = 'login.html';
        loginLink.style.background = 'var(--primary-color)';
        return;
    }

    loginLink.textContent = `Merhaba, ${user}`;
    loginLink.href = '#';
    loginLink.style.background = 'transparent';
    loginLink.style.border = '1px solid var(--glass-border)';

    if (localStorage.getItem('role') === 'Student') {
        const myCourses = document.createElement('li');
        myCourses.id = 'my-courses-nav';
        myCourses.innerHTML = '<a href="my-courses.html">Kurslarım</a>';
        navList.appendChild(myCourses);
    }

    const logoutItem = document.createElement('li');
    logoutItem.id = 'logout-btn-nav';
    const logoutButton = document.createElement('a');
    logoutButton.href = '#';
    logoutButton.textContent = 'Çıkış Yap';
    logoutButton.className = 'view-btn';
    logoutButton.style.background = 'rgba(255, 50, 50, 0.2)';
    logoutButton.addEventListener('click', (event) => {
        event.preventDefault();
        localStorage.clear();
        window.location.reload();
    });
    logoutItem.appendChild(logoutButton);
    navList.appendChild(logoutItem);
}

async function loadHomeInstructors(): Promise<void> {
    const list = document.getElementById('instructor-list');
    if (!list) {
        return;
    }

    try {
        const instructors = await getInstructors();
        list.innerHTML = '';
        if (instructors.length === 0) {
            list.textContent = 'Henüz eğitmen bulunmuyor.';
            return;
        }

        instructors.forEach((instructor) => list.appendChild(createInstructorCard(instructor)));
    } catch (error) {
        console.error(error);
        list.textContent = 'Eğitmenler yüklenemedi.';
    }
}

function createInstructorCard(instructor: Instructor): HTMLElement {
    const card = document.createElement('div');
    card.className = 'instructor-card-home';

    const image = document.createElement('img');
    image.src = getAssetUrl(instructor.headShotUrl) || 'https://via.placeholder.com/150?text=Eğitmen';
    image.alt = instructor.fullName;
    card.append(image);

    const name = document.createElement('h3');
    name.textContent = instructor.fullName;
    card.append(name);

    const bio = document.createElement('p');
    bio.style.cssText = 'color:#aaa; font-size:0.9rem';
    bio.textContent = instructor.bio ?? '';
    card.append(bio);
    return card;
}

async function loadCourses(): Promise<void> {
    const courseList = document.getElementById('course-list');
    if (!courseList) {
        return;
    }

    try {
        const courses = await getCourses();
        courseList.innerHTML = '';
        if (courses.length === 0) {
            courseList.textContent = 'Şu an kurs bulunmuyor.';
            return;
        }
        courses.forEach((course) => courseList.appendChild(createCourseCard(course)));
    } catch (error) {
        console.error(error);
        courseList.textContent = 'Kurslar yüklenemedi. Lütfen backend servisini kontrol edin.';
    }
}

function createCourseCard(course: Course): HTMLElement {
    const card = document.createElement('div');
    card.className = 'course-card';

    const image = document.createElement('div');
    image.className = 'course-image';
    const imageUrl = getAssetUrl(course.imageUrl);
    if (imageUrl) {
        image.style.backgroundImage = `url("${imageUrl}")`;
    } else {
        image.style.background = 'linear-gradient(135deg, #1e293b, #334155)';
    }
    card.append(image);

    const content = document.createElement('div');
    content.className = 'course-content';
    content.innerHTML = `
        <div class="course-category"></div>
        <h3 class="course-title"></h3>
        <div class="course-instructor"><span></span></div>
        <div class="course-footer">
            <span class="course-price"></span>
            <a class="view-btn">Detayları Gör</a>
        </div>
    `;
    content.querySelector('.course-category')!.textContent = course.categoryName;
    content.querySelector('.course-title')!.textContent = course.title;
    content.querySelector('.course-instructor span')!.textContent = `${course.instructorName} Tarafından`;
    if (course.description) {
        const description = document.createElement('p');
        description.textContent = course.description;
        description.style.cssText =
            'font-size:0.9rem;color:#94a3b8;margin-bottom:1rem;display:-webkit-box;-webkit-line-clamp:2;-webkit-box-orient:vertical;overflow:hidden;';
        content.insertBefore(description, content.querySelector('.course-footer'));
    }
    content.querySelector('.course-price')!.textContent = `${course.price.toFixed(2)} TL`;
    const detailsLink = content.querySelector<HTMLAnchorElement>('.view-btn')!;
    detailsLink.href = `course-details.html?id=${course.id}`;
    card.append(content);
    return card;
}
