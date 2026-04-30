const baseUrl = '/api';

async function request(path, options = {}) {
  const token = localStorage.getItem('gymproject_token');
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {}),
    ...(token ? { Authorization: `Bearer ${token}` } : {})
  };

  const response = await fetch(`${baseUrl}${path}`, {
    ...options,
    headers,
    body: options.body ? JSON.stringify(options.body) : undefined
  });

  const data = await response.json().catch(() => null);
  if (!response.ok || (data && data.success === false)) {
    const error = data?.message || data?.errors?.[0]?.errorMessage || response.statusText;
    throw new Error(error || 'Request failed');
  }

  return data?.data ?? data;
}

export async function login(credentials) {
  return request('/Auth/login', { method: 'POST', body: credentials });
}

export async function register(credentials) {
  return request('/Auth/register', { method: 'POST', body: credentials });
}

export async function fetchUsers() {
  return request('/User/users');
}

export async function fetchProfile() {
  return request('/UserProfiles/GetMyProfile');
}

export async function createProfile(profile) {
  return request('/UserProfiles/CreateUserProfile', { method: 'POST', body: profile });
}

export async function updateProfile(profile) {
  return request('/UserProfiles/UpdateUserProfile', { method: 'PUT', body: profile });
}

export async function createUserWithRole(user, role) {
  return request(`/Auth/registerUserWithRole?role=${encodeURIComponent(role)}`, { method: 'POST', body: user });
}

export async function reactivateUser(userId) {
  return request(`/User/ReactivateUser/${userId}`, { method: 'PUT' });
}

export async function deactivateUser(userId) {
  return request(`/User/${userId}`, { method: 'DELETE' });
}

export async function updateUserRole(userId, role) {
  return request(`/User/${userId}/role`, { method: 'PUT', body: { role } });
}

export async function fetchExercises() {
  return request('/Exercises/GetAllExercises');
}

export async function fetchExercisesByMuscleGroup(muscleGroup) {
  return request(`/Exercises/GetExercisesByMuscleGroup?muscleGroup=${encodeURIComponent(muscleGroup)}`);
}

export async function createExercise(exercise) {
  return request('/Exercises/CreateExercise', { method: 'POST', body: exercise });
}

export async function fetchFitnessPlans() {
  return request('/FitnessPlan/GetAllFitnessPlans');
}

export async function createFitnessPlan(plan) {
  return request('/FitnessPlan/CreateFitnessPlan', { method: 'POST', body: plan });
}

export async function updateFitnessPlan(id, plan) {
  return request(`/FitnessPlan/UpdateFitnessPlan/${id}`, { method: 'PUT', body: plan });
}

export async function deleteFitnessPlan(id) {
  return request(`/FitnessPlan/DeleteFitnessPlan/${id}`, { method: 'DELETE' });
}

export async function fetchFitnessPlanDetails(id) {
  return request(`/FitnessPlan/${id}/details`);
}

export async function createPlanExercise(planExercise) {
  return request('/PlanExercises', { method: 'POST', body: planExercise });
}

export async function fetchPlanExercises() {
  return request('/PlanExercises');
}

export async function deletePlanExercise(id) {
  return request(`/PlanExercises/${id}`, { method: 'DELETE' });
}

export async function fetchMeals() {
  return request('/Meal/GetAllMeals');
}

export async function createMeal(meal) {
  return request('/Meal/CreateMeal', { method: 'POST', body: meal });
}
