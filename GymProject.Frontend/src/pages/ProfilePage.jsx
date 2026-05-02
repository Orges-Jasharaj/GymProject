import { useEffect, useState } from 'react';
import { fetchProfile, createProfile, updateProfile, fetchActiveUserSubscription } from '../api';
import { useAuth } from '../hooks/useAuth';

export default function ProfilePage() {
  const { auth } = useAuth();
  const [profile, setProfile] = useState(null);
  const [activeSubscription, setActiveSubscription] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [form, setForm] = useState({ gender: '', heightCm: '', currentWeightKg: '', goalWeightKg: '', activityLevel: '', fitnessGoal: '' });
  const [message, setMessage] = useState(null);

  useEffect(() => {
    if (!auth?.token) return;

    const loadProfile = async () => {
      try {
        const result = await fetchProfile();
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

        if (result?.userId) {
          try {
            const active = await fetchActiveUserSubscription(result.userId);
            setActiveSubscription(active);
          } catch {
            setActiveSubscription(null);
          }
        }
      } catch (err) {
        setMessage(err.message);
      }
    };

    loadProfile();
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

  const subscriptionName = activeSubscription?.subscriptionPlanName ?? profile?.activeSubscriptionPlanName;
  const subscriptionActive = activeSubscription?.isActive ?? profile?.activeSubscriptionIsActive;
  const subscriptionStartDate = activeSubscription?.startDate ?? profile?.activeSubscriptionStartDate;
  const subscriptionEndDate = activeSubscription?.endDate ?? profile?.activeSubscriptionEndDate;
  const hasSubscription = Boolean(subscriptionName);

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
            <div className="profile-card-header">
              <div>
                <p className="profile-card-eyebrow">Profile overview</p>
                <h2>Your fitness profile</h2>
              </div>
              <span className={`profile-badge ${hasSubscription ? 'active' : 'inactive'}`}>
                {hasSubscription ? 'Active subscription' : 'No active subscription'}
              </span>
            </div>

            <div className="profile-grid">
              <div className="profile-stat">
                <span>Gender</span>
                <strong>{profile.gender ?? 'Not set'}</strong>
              </div>
              <div className="profile-stat">
                <span>Height</span>
                <strong>{profile.heightCm ?? 'N/A'} cm</strong>
              </div>
              <div className="profile-stat">
                <span>Current weight</span>
                <strong>{profile.currentWeightKg ?? 'N/A'} kg</strong>
              </div>
              <div className="profile-stat">
                <span>Goal weight</span>
                <strong>{profile.goalWeightKg ?? 'N/A'} kg</strong>
              </div>
              <div className="profile-stat">
                <span>Activity level</span>
                <strong>{profile.activityLevel ?? 'Not set'}</strong>
              </div>
              <div className="profile-stat">
                <span>Fitness goal</span>
                <strong>{profile.fitnessGoal ?? 'Not set'}</strong>
              </div>
            </div>

            <div className="profile-details">
              <p className="profile-details-title">Subscription details</p>
              <div className="profile-detail-item">
                <span>Subscription plan</span>
                <strong>{subscriptionName ?? 'None'}</strong>
              </div>
              {hasSubscription && (
                <>
                  <div className="profile-detail-item">
                    <span>Status</span>
                    <strong>{subscriptionActive ? 'Active' : 'Expired'}</strong>
                  </div>
                  <div className="profile-detail-item">
                    <span>Start date</span>
                    <strong>{subscriptionStartDate ? new Date(subscriptionStartDate).toLocaleDateString() : 'N/A'}</strong>
                  </div>
                  <div className="profile-detail-item">
                    <span>End date</span>
                    <strong>{subscriptionEndDate ? new Date(subscriptionEndDate).toLocaleDateString() : 'N/A'}</strong>
                  </div>
                </>
              )}
            </div>
          </div>

          <button type="button" className="primary-button" onClick={startEditing}>Edit profile</button>
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
