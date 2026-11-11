import { getAssetUrl, getInstructors } from './api.js';
document.addEventListener('DOMContentLoaded', () => {
    const user = localStorage.getItem('username');
    const button = document.getElementById('nav-auth-btn');
    if (button && user) {
        button.textContent = `Merhaba, ${user}`;
        button.href = '#';
    }
    else if (button) {
        button.href = 'login.html';
    }
    void loadInstructors();
});
async function loadInstructors() {
    const container = document.getElementById('instructors-container');
    try {
        const instructors = await getInstructors();
        container.innerHTML = '';
        if (instructors.length === 0) {
            container.textContent = 'Henüz eğitmen bulunmuyor.';
            return;
        }
        instructors.forEach((instructor) => {
            const card = document.createElement('div');
            card.className = 'instructor-card';
            card.innerHTML = '<img class="instructor-img"><h3 class="instructor-name"></h3><p class="instructor-bio"></p>';
            const image = card.querySelector('img');
            image.src = getAssetUrl(instructor.headShotUrl) || 'https://via.placeholder.com/150?text=Instructor';
            image.alt = instructor.fullName;
            card.querySelector('.instructor-name').textContent = instructor.fullName;
            card.querySelector('.instructor-bio').textContent =
                instructor.bio || 'Henüz biyografi eklenmemiş.';
            container.appendChild(card);
        });
    }
    catch (error) {
        console.error(error);
        container.textContent = 'Eğitmenler yüklenemedi.';
    }
}
