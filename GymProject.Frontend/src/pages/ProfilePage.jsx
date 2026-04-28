import { useEffect, useState } from 'react';
import { fetchProfile, createProfile, updateProfile } from '../api';
import { useAuth } from '../hooks/useAuth';

export default function ProfilePage() {
  const { auth } = useAuth();
  const [profile, setProfile] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [form, setForm] = useState({ gender: '', heightCm: '', currentWeightKg: '', goalWeightKg: '', activityLevel: '', fitnessGoal: '' });
  const [message, setMessage] = useState(null);

  useEffect(() => {
    if (!auth?.token) return;
    fetchProfile()
      .then((result) => {
        setProfile(result);
        if (result) {
          setForm({
            gender: result.gender ?? '',
            heightCm: result.heightCm?.toString() ?? '',
            currentWeightKg: result.currentWeightKg?.toString() ?? '',
            goalWeightKg: result.goalWeightKg?.toString() ?? '',
            activityLevel: result.activityLevel ?? '',
            fitnessGoal: result.fitnessGoal ?? ''
          });
        }
      })
      .catch((err) => setMessage(err.message));
  }, [auth]);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    try {
      const profilePayload = {
        gender: form.gender,
        heightCm: form.heightCm ? Number(form.heightCm) : null,
        currentWeightKg: form.currentWeightKg ? Number(form.currentWeightKg) : null,
        goalWeightKg: form.goalWeightKg ? Number(form.goalWeightKg) : null,
        activityLevel: form.activityLevel,
        fitnessGoal: form.fitnessGoal
      };

      if (profile) {
        await updateProfile(profilePayload);
        setMessage('Profile updated successfully.');
      } else {
        await createProfile(profilePayload);
        setMessage('Profile created successfully.');
      }

      const refreshed = await fetchProfile();
      setProfile(refreshed);
      setIsEditing(false);
    } catch (error) {
      setMessage(error.message);
    }
  };

  const startEditing = () => {
    setForm({
      gender: profile?.gender ?? '',
      heightCm: profile?.heightCm?.toString() ?? '',
      currentWeightKg: profile?.currentWeightKg?.toString() ?? '',
      goalWeightKg: profile?.goalWeightKg?.toString() ?? '',
      activityLevel: profile?.activityLevel ?? '',
      fitnessGoal: profile?.fitnessGoal ?? ''
    });
    setIsEditing(true);
    setMessage(null);
  };

  const cancelEditing = () => {
    setIsEditing(false);
    setMessage(null);
  };

  if (!auth?.token) {
    return (
      <section className="page page-data">
        <h1>Profile</h1>
        <p>Please log in to see or create your profile.</p>
      </section>
    );
  }

  return (
    <section className="page page-data">
      <h1>Profile</h1>
      {message && <p className="form-message">{message}</p>}
      {profile && !isEditing ? (
        <>
          <div className="profile-card">
            <p><strong>Gender:</strong> {profile.gender ?? 'Not set'}</p>
            <p><strong>Height:</strong> {profile.heightCm ?? 'N/A'} cm</p>
            <p><strong>Current weight:</strong> {profile.currentWeightKg ?? 'N/A'} kg</p>
            <p><strong>Goal weight:</strong> {profile.goalWeightKg ?? 'N/A'} kg</p>
            <p><strong>Activity level:</strong> {profile.activityLevel ?? 'Not set'}</p>
            <p><strong>Fitness goal:</strong> {profile.fitnessGoal ?? 'Not set'}</p>
          </div>
          <button type="button" onClick={startEditing}>Edit profile</button>
        </>
      ) : (
        <form className="entity-form" onSubmit={handleSubmit}>
          <label>
            Gender
            <select name="gender" value={form.gender} onChange={handleChange}>
              <option value="" disabled>Select gender</option>
              <option value="M">Male</option>
              <option value="F">Female</option>
            </select>
          </label>
          <label>
            Height (cm)
            <input name="heightCm" type="number" value={form.heightCm} onChange={handleChange} />
          </label>
          <label>
            Current weight (kg)
            <input name="currentWeightKg" type="number" value={form.currentWeightKg} onChange={handleChange} />
          </label>
          <label>
            Goal weight (kg)
            <input name="goalWeightKg" type="number" value={form.goalWeightKg} onChange={handleChange} />
          </label>
          <label>
            Activity level
            <select name="activityLevel" value={form.activityLevel} onChange={handleChange}>
              <option value="" disabled>Select activity level</option>
              <option value="low">Low</option>
              <option value="moderately active">Moderately active</option>
              <option value="very active">Very active</option>
            </select>
          </label>
          <label>
            Fitness goal
            <select name="fitnessGoal" value={form.fitnessGoal} onChange={handleChange}>
              <option value="" disabled>Select fitness goal</option>
              <option value="lose weight">Lose weight</option>
              <option value="build muscle">Build muscle</option>
              <option value="gain weight">Gain weight</option>
            </select>
          </label>
          <div className="form-actions">
            <button type="submit">{profile ? 'Update profile' : 'Create profile'}</button>
            {profile && <button type="button" onClick={cancelEditing}>Cancel</button>}
          </div>
        </form>
      )}
    </section>
  );
}
