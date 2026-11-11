export const API_BASE_URL = '/api';
function getToken() {
    return localStorage.getItem('token');
}
async function getErrorMessage(response) {
    const body = await response.text();
    if (!body) {
        return `İstek başarısız oldu (${response.status}).`;
    }
    try {
        const error = JSON.parse(body);
        const validationErrors = error.errors
            ? Object.values(error.errors).flat().join(' ')
            : '';
        return validationErrors || error.message || error.title || body;
    }
    catch {
        return body;
    }
}
export async function apiFetch(path, init = {}) {
    const headers = new Headers(init.headers);
    const token = getToken();
    if (token) {
        headers.set('Authorization', 'Bearer ' + token);
    }
    if (init.body && !(init.body instanceof FormData)) {
        headers.set('Content-Type', 'application/json');
    }
    const response = await fetch(`${API_BASE_URL}${path}`, {
        ...init,
        headers,
    });
    if (!response.ok) {
        throw new Error(await getErrorMessage(response));
    }
    if (response.status === 204) {
        return undefined;
    }
    return response.json();
}
export function getAssetUrl(path) {
    if (!path) {
        return '';
    }
    return path.startsWith('/') ? `${window.location.origin}${path}` : path;
}
export function login(username, password) {
    return apiFetch('/auth/login', {
        method: 'POST',
        body: JSON.stringify({ username, password }),
    });
}
export function register(formData) {
    return apiFetch('/auth/register', {
        method: 'POST',
        body: formData,
    });
}
export function getCourses() {
    return apiFetch('/courses');
}
export function getInstructors() {
    return apiFetch('/instructors');
}
export function getCourse(id) {
    return apiFetch(`/courses/${id}`);
}
export function getLessons(courseId) {
    return apiFetch(`/Lesson/course/${courseId}`);
}
export function checkEnrollment(courseId) {
    return apiFetch(`/Enrollments/check/${courseId}`);
}
export function enroll(courseId) {
    return apiFetch(`/Enrollments/${courseId}`, { method: 'POST' });
}
export function getProgress(courseId) {
    return apiFetch(`/Enrollments/progress/${courseId}`);
}
export function updateProgress(courseId, progress) {
    return apiFetch(`/Enrollments/progress/${courseId}`, {
        method: 'POST',
        body: JSON.stringify(progress),
    });
}
export function getInstructorCourses() {
    return apiFetch('/courses/my-courses');
}
export function getInstructorStats() {
    return apiFetch('/courses/stats');
}
export function createCourse(course) {
    return apiFetch('/courses', {
        method: 'POST',
        body: JSON.stringify(course),
    }).then((response) => response.data);
}
export function updateCourse(id, course) {
    return apiFetch(`/courses/${id}`, {
        method: 'PUT',
        body: JSON.stringify(course),
    });
}
export function deleteCourse(id) {
    return apiFetch(`/courses/${id}`, { method: 'DELETE' });
}
export function toggleCourse(id) {
    return apiFetch(`/courses/${id}/toggle`, { method: 'POST' });
}
export function uploadFile(kind, file) {
    const formData = new FormData();
    formData.append('file', file);
    return apiFetch(`/Upload/${kind}`, {
        method: 'POST',
        body: formData,
    });
}
export function getAdminPendingInstructors() {
    return apiFetch('/Admin/pending-instructors');
}
export function getAdminPendingCourses() {
    return apiFetch('/Admin/pending-courses');
}
export function getAdminCourses() {
    return apiFetch('/Admin/all-courses');
}
export function getAdminUsers() {
    return apiFetch('/Admin/all-users');
}
export function adminAction(path, method = 'POST') {
    return apiFetch(path, { method });
}
export function createLesson(lesson) {
    return apiFetch('/Lesson', {
        method: 'POST',
        body: JSON.stringify(lesson),
    }).then((response) => response.data);
}
export function updateLesson(id, lesson) {
    return apiFetch(`/Lesson/${id}`, {
        method: 'PUT',
        body: JSON.stringify(lesson),
    });
}
export function deleteLesson(id) {
    return apiFetch(`/Lesson/${id}`, { method: 'DELETE' });
}
