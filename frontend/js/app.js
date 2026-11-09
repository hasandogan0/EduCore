const API_BASE_URL = 'http://localhost:5239/api';

document.addEventListener('DOMContentLoaded', () => {
    updateNav();
    loadCourses();
    loadHomeInstructors();
});

// ... updateNav and loadCourses codes remain same ...

async function loadHomeInstructors() {
    const list = document.getElementById('instructor-list');
    if (!list) return;

    try {
        const res = await fetch(`${API_BASE_URL}/Instructors`);
        if (!res.ok) throw new Error('Eğitmenler yüklenemedi');

        const instructors = await res.json();
        list.innerHTML = '';

        if (instructors.length === 0) {
            list.innerHTML = '<p>Henüz eğitmen bulunmuyor.</p>';
            return;
        }

        instructors.forEach(inst => {
            let img = inst.headshotUrl || 'https://via.placeholder.com/150?text=Eğitmen';
            // Fix URL
            if (img.startsWith('/')) {
                img = API_BASE_URL.replace('/api', '') + img;
            }

            const div = document.createElement('div');
            div.className = 'instructor-card-home';
            div.innerHTML = `
                <img src="${img}" alt="${inst.fullName}">
                <h3>${inst.fullName}</h3>
                <p style="color:#aaa; font-size:0.9rem">${inst.bio || ''}</p>
            `;
            list.appendChild(div);
        });

    } catch (err) {
        console.error(err);
        list.innerHTML = '<p>Eğitmenler yüklenemedi.</p>';
    }
}

function updateNav() {
    const user = localStorage.getItem('username');
    const navList = document.querySelector('nav ul');
    const loginLink = navList.querySelector('.view-btn');

    if (!navList || !loginLink) return;

    if (user) {
        // User is logged in
        loginLink.textContent = `Merhaba, ${user}`;
        loginLink.href = '#';
        loginLink.style.background = 'transparent';
        loginLink.style.border = '1px solid var(--glass-border)';

        // Remove existing logout btn if any to avoid duplicates
        const existingLogout = document.getElementById('logout-btn-nav');
        if (existingLogout) existingLogout.remove();

        // Add 'Kurslarım' link if Student
        const role = localStorage.getItem('role');
        const existingMyCourses = document.getElementById('my-courses-nav');
        if (role === 'Student' && !existingMyCourses) {
            const li = document.createElement('li');
            li.id = 'my-courses-nav';
            li.innerHTML = '<a href="my-courses.html">Kurslarım</a>';
            // Insert before Logout or at end? Nav order: Home, Courses, [My Courses], User, Logout
            // Easy way: just append to navList
            navList.appendChild(li);
        }

        // Add Logout button
        const logoutLi = document.createElement('li');
        logoutLi.id = 'logout-btn-nav'; // Mark it to avoid duplicates
        const logoutBtn = document.createElement('a');
        logoutBtn.href = '#';
        logoutBtn.textContent = 'Çıkış Yap';
        logoutBtn.className = 'view-btn';
        logoutBtn.style.background = 'rgba(255, 50, 50, 0.2)';

        logoutBtn.addEventListener('click', (e) => {
            e.preventDefault();
            localStorage.clear();
            window.location.reload();
        });

        logoutLi.appendChild(logoutBtn);
        navList.appendChild(logoutLi);
    } else {
        // User is passed "Giriş Yap" in HTML, but let's ensure functionality
        loginLink.textContent = 'Giriş Yap';
        loginLink.href = 'login.html';
        loginLink.style.background = 'var(--primary-color)';

        // Remove logout if present logic not really needed if we reload on logout, but safe to have
        const existingLogout = document.getElementById('logout-btn-nav');
        if (existingLogout) existingLogout.remove();
    }
}

async function loadCourses() {
    const courseList = document.getElementById('course-list');
    if (!courseList) return;

    try {
        const response = await fetch(`${API_BASE_URL}/courses`);
        if (!response.ok) {
            throw new Error('Kurslar getirilemedi');
        }

        const courses = await response.json();

        courseList.innerHTML = ''; // Clear loading

        if (courses.length === 0) {
            courseList.innerHTML = '<p>Şu an kurs bulunmuyor.</p>';
            return;
        }

        courses.forEach(course => {
            const card = document.createElement('div');
            card.className = 'course-card';

            let imgUrl = course.imageUrl;
            if (imgUrl && imgUrl.startsWith('/')) {
                imgUrl = API_BASE_URL.replace('/api', '') + imgUrl;
            }

            // Random gradient placeholder if no image
            const bgStyle = imgUrl
                ? `background-image: url('${imgUrl}')`
                : `background: linear-gradient(135deg, #1e293b, #334155)`;

            // For the <img> tag inside
            const imgTag = imgUrl ? `<img src="${imgUrl}" alt="${course.title}" style="width:100%;height:100%;object-fit:cover;">` : '';

            card.innerHTML = `
                <div class="course-image" style="${bgStyle}">
                    ${imgTag}
                </div>
                <div class="course-content">
                    <div class="course-category">${course.categoryName}</div>
                    <h3 class="course-title">${course.title}</h3>
                    <div class="course-instructor">
                        <span>${course.instructorName} Tarafından</span>
                    </div>
                    ${course.description ? `<p style="font-size:0.9rem;color:#94a3b8;margin-bottom:1rem;display:-webkit-box;-webkit-line-clamp:2;-webkit-box-orient:vertical;overflow:hidden;">${course.description}</p>` : ''}
                    <div class="course-footer">
                        <span class="course-price">${course.price.toFixed(2)} TL</span>
                        <a href="course-details.html?id=${course.id}" class="view-btn">Detayları Gör</a>
                    </div>
                </div>
            `;

            courseList.appendChild(card);
        });
    } catch (error) {
        console.error('Error:', error);
        if (courseList) courseList.innerHTML = '<p style="color:red">Kurslar yüklenemedi. Lütfen backend servisini kontrol edin.</p>';
    }
}
