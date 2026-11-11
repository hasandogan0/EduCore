import type {
    ApiErrorResponse,
    AdminCourse,
    AdminUser,
    AuthResponse,
    Course,
    CreateCourse,
    Instructor,
    InstructorStats,
    Lesson,
    UploadResponse,
} from './types.js';

export const API_BASE_URL = '/api';

function getToken(): string | null {
    return localStorage.getItem('token');
}

async function getErrorMessage(response: Response): Promise<string> {
    const body = await response.text();
    if (!body) {
        return `İstek başarısız oldu (${response.status}).`;
    }

    try {
        const error = JSON.parse(body) as ApiErrorResponse;
        const validationErrors = error.errors
            ? Object.values(error.errors).flat().join(' ')
            : '';
        return validationErrors || error.message || error.title || body;
    } catch {
        return body;
    }
}

export async function apiFetch<T>(
    path: string,
    init: RequestInit = {},
): Promise<T> {
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
        return undefined as T;
    }

    return response.json() as Promise<T>;
}

export function getAssetUrl(path: string | null | undefined): string {
    if (!path) {
        return '';
    }

    return path.startsWith('/') ? `${window.location.origin}${path}` : path;
}

export function login(username: string, password: string): Promise<AuthResponse> {
    return apiFetch<AuthResponse>('/auth/login', {
        method: 'POST',
        body: JSON.stringify({ username, password }),
    });
}

export function register(formData: FormData): Promise<AuthResponse> {
    return apiFetch<AuthResponse>('/auth/register', {
        method: 'POST',
        body: formData,
    });
}

export function getCourses(): Promise<Course[]> {
    return apiFetch<Course[]>('/courses');
}

export function getInstructors(): Promise<Instructor[]> {
    return apiFetch<Instructor[]>('/instructors');
}

export function getCourse(id: number): Promise<Course> {
    return apiFetch<Course>(`/courses/${id}`);
}

export function getLessons(courseId: number): Promise<Lesson[]> {
    return apiFetch<Lesson[]>(`/Lesson/course/${courseId}`);
}

export function checkEnrollment(courseId: number): Promise<boolean> {
    return apiFetch<boolean>(`/Enrollments/check/${courseId}`);
}

export function enroll(courseId: number): Promise<{ message: string }> {
    return apiFetch<{ message: string }>(`/Enrollments/${courseId}`, { method: 'POST' });
}

export function getProgress(courseId: number): Promise<number> {
    return apiFetch<number>(`/Enrollments/progress/${courseId}`);
}

export function updateProgress(courseId: number, progress: number): Promise<{ message: string }> {
    return apiFetch<{ message: string }>(`/Enrollments/progress/${courseId}`, {
        method: 'POST',
        body: JSON.stringify(progress),
    });
}

export function getInstructorCourses(): Promise<Course[]> {
    return apiFetch<Course[]>('/courses/my-courses');
}

export function getInstructorStats(): Promise<InstructorStats> {
    return apiFetch<InstructorStats>('/courses/stats');
}

export function createCourse(course: CreateCourse): Promise<Course> {
    return apiFetch<{ data: Course }>('/courses', {
        method: 'POST',
        body: JSON.stringify(course),
    }).then((response) => response.data);
}

export function updateCourse(id: number, course: CreateCourse): Promise<void> {
    return apiFetch<void>(`/courses/${id}`, {
        method: 'PUT',
        body: JSON.stringify(course),
    });
}

export function deleteCourse(id: number): Promise<void> {
    return apiFetch<void>(`/courses/${id}`, { method: 'DELETE' });
}

export function toggleCourse(id: number): Promise<void> {
    return apiFetch<void>(`/courses/${id}/toggle`, { method: 'POST' });
}

export function uploadFile(kind: 'image' | 'video', file: File): Promise<UploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return apiFetch<UploadResponse>(`/Upload/${kind}`, {
        method: 'POST',
        body: formData,
    });
}

export function getAdminPendingInstructors(): Promise<AdminUser[]> {
    return apiFetch<AdminUser[]>('/Admin/pending-instructors');
}

export function getAdminPendingCourses(): Promise<Course[]> {
    return apiFetch<Course[]>('/Admin/pending-courses');
}

export function getAdminCourses(): Promise<AdminCourse[]> {
    return apiFetch<AdminCourse[]>('/Admin/all-courses');
}

export function getAdminUsers(): Promise<AdminUser[]> {
    return apiFetch<AdminUser[]>('/Admin/all-users');
}

export function adminAction(path: string, method: 'POST' | 'DELETE' = 'POST'): Promise<void> {
    return apiFetch<void>(path, { method });
}

export function createLesson(lesson: {
    title: string;
    videoUrl: string | null;
    courseId: number;
}): Promise<Lesson> {
    return apiFetch<{ data: Lesson }>('/Lesson', {
        method: 'POST',
        body: JSON.stringify(lesson),
    }).then((response) => response.data);
}

export function updateLesson(id: number, lesson: {
    title: string;
    videoUrl: string | null;
    courseId: number;
}): Promise<void> {
    return apiFetch<void>(`/Lesson/${id}`, {
        method: 'PUT',
        body: JSON.stringify(lesson),
    });
}

export function deleteLesson(id: number): Promise<void> {
    return apiFetch<void>(`/Lesson/${id}`, { method: 'DELETE' });
}
