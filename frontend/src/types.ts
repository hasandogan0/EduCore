export interface Course {
    id: number;
    title: string;
    description: string;
    price: number;
    imageUrl: string;
    quota: number;
    status: string;
    categoryName: string;
    instructorName: string;
    instructorId: string;
    isActive: boolean;
}

export interface Lesson {
    id: number;
    title: string;
    videoUrl: string | null;
    courseId: number;
}

export interface CreateCourse {
    title: string;
    description: string;
    price: number;
    imageUrl: string | null;
    quota: number;
    categoryId: number;
}

export interface InstructorStats {
    totalEarnings: number;
    totalStudents: number;
    totalCourses: number;
}

export interface UploadResponse {
    url: string;
    message: string;
}

export interface AdminCourse {
    id: number;
    title: string;
    instructor: string;
    price: number;
    status: string;
    isActive: boolean;
    totalEarnings: number;
}

export interface AdminUser {
    id: string;
    username: string;
    fullName: string;
    email: string;
    role: string;
    isActive: boolean;
    totalEarnings: number;
    headShotUrl: string;
}

export interface Instructor {
    id: string;
    fullName: string;
    username: string;
    bio: string | null;
    headShotUrl: string | null;
}

export interface AuthResponse {
    token: string;
    username: string;
    email: string;
    role: string;
    fullName: string;
    headShotUrl: string;
}

export interface ApiErrorResponse {
    message?: string;
    title?: string;
    errors?: Record<string, string[]>;
}
