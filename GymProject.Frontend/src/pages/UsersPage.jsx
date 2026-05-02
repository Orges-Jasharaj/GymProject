import { useEffect, useState } from 'react';
import { fetchUsers, createUserWithRole, reactivateUser, deactivateUser, updateUserRole, fetchSubscriptionPlans, createSubscriptionPlan, subscribeUserToPlan, fetchActiveUserSubscription, fetchGyms, createGym } from '../api';
import { useAuth } from '../hooks/useAuth';

const roleOptions = [
  { value: 'User', label: 'User' },
  { value: 'Admin', label: 'Admin' },
  { value: 'SuperAdmin', label: 'SuperAdmin' }
];

export default function UsersPage() {
  const { auth } = useAuth();
  const [users, setUsers] = useState([]);
  const [error, setError] = useState(null);
  const [createError, setCreateError] = useState(null);
  const [createSuccess, setCreateSuccess] = useState(null);
  const [actionError, setActionError] = useState(null);
  const [actionSuccess, setActionSuccess] = useState(null);
  const [editRoles, setEditRoles] = useState({});
  const [newUser, setNewUser] = useState({ firstName: '', lastName: '', dateOfBirth: '', email: '', password: '', role: 'User' });
  const [subscriptionPlans, setSubscriptionPlans] = useState([]);
  const [subscriptionError, setSubscriptionError] = useState(null);
  const [subscriptionSuccess, setSubscriptionSuccess] = useState(null);
  const [selectedSubscriptionPlans, setSelectedSubscriptionPlans] = useState({});
  const [newSubscriptionPlan, setNewSubscriptionPlan] = useState({ gymId: '', name: '', price: '', durationInDays: '' });
  const [createSubscriptionError, setCreateSubscriptionError] = useState(null);
  const [createSubscriptionSuccess, setCreateSubscriptionSuccess] = useState(null);
  const [gyms, setGyms] = useState([]);
  const [gymError, setGymError] = useState(null);
  const [gymSuccess, setGymSuccess] = useState(null);
  const [newGym, setNewGym] = useState({ name: '', city: '' });
  const [activeSubscriptions, setActiveSubscriptions] = useState({});
  const [activeAdminTab, setActiveAdminTab] = useState('user');

  const isSuperAdmin = auth?.roles?.includes('SuperAdmin');
  const canViewUsers = isSuperAdmin;

  useEffect(() => {
    setError(null);
    setSubscriptionError(null);
    if (!auth?.token || !isSuperAdmin) return;

    fetchUsers()
      .then(async (users) => {
        setUsers(users);
        const subs = await Promise.all(users.map(async (user) => {
          try {
            const subscription = await fetchActiveUserSubscription(user.id);
            return { userId: user.id, subscription };
          } catch {
            return null;
          }
        }));

        setActiveSubscriptions(subs.reduce((map, item) => {
          if (item?.subscription) {
            map[item.userId] = item.subscription;
          }
          return map;
        }, {}));
      })
      .catch((err) => setError(err.message));

    fetchSubscriptionPlans()
      .then(setSubscriptionPlans)
      .catch((err) => setSubscriptionError(err.message));

    fetchGyms()
      .then(setGyms)
      .catch((err) => setGymError(err.message));
  }, [auth, canViewUsers, isSuperAdmin]);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setNewUser((current) => ({ ...current, [name]: value }));
  };

  const handleRoleSelection = (userId, value) => {
    setEditRoles((current) => ({ ...current, [userId]: value }));
  };

  const handleSaveRole = async (user) => {
    setActionError(null);
    setActionSuccess(null);

    try {
      const selectedRole = editRoles[user.id] || user.roles?.[0] || 'User';
      await updateUserRole(user.id, selectedRole);
      setActionSuccess(`${user.email} roli u ruajt si ${selectedRole}`);
      setUsers(await fetchUsers());
    } catch (err) {
      setActionError(err.message);
    }
  };

  const handleToggleActive = async (user) => {
    setActionError(null);
    setActionSuccess(null);

    try {
      const selectedRole = editRoles[user.id] || user.roles?.[0] || 'User';
      if (user.isActive) {
        await deactivateUser(user.id);
        setActionSuccess(`${user.email} u deaktivizua`);
      } else {
        await reactivateUser(user.id);
        setActionSuccess(`${user.email} u aktivizua`);
      }

      await updateUserRole(user.id, selectedRole);
      setEditRoles((current) => ({ ...current, [user.id]: selectedRole }));
      setUsers(await fetchUsers());
    } catch (err) {
      setActionError(err.message);
    }
  };

  const handleCreateUser = async (event) => {
    event.preventDefault();
    setCreateError(null);
    setCreateSuccess(null);

    try {
      await createUserWithRole({
        firstName: newUser.firstName,
        lastName: newUser.lastName,
        dateOfBirth: newUser.dateOfBirth,
        email: newUser.email,
        password: newUser.password
      }, newUser.role);

      setCreateSuccess(`User created successfully as ${newUser.role}`);
      setNewUser({ firstName: '', lastName: '', dateOfBirth: '', email: '', password: '', role: 'User' });
      fetchUsers().then(setUsers).catch((err) => setError(err.message));
    } catch (err) {
      setCreateError(err.message);
    }
  };

  const handleSubscriptionChange = (event) => {
    const { name, value } = event.target;
    setNewSubscriptionPlan((current) => ({ ...current, [name]: value }));
  };

  const handleCreateSubscriptionPlan = async (event) => {
    event.preventDefault();
    setCreateSubscriptionError(null);
    setCreateSubscriptionSuccess(null);

    try {
      await createSubscriptionPlan({
        gymId: newSubscriptionPlan.gymId,
        name: newSubscriptionPlan.name,
        price: Number(newSubscriptionPlan.price),
        durationInDays: Number(newSubscriptionPlan.durationInDays)
      });
      setCreateSubscriptionSuccess('Subscription plan created successfully.');
      setNewSubscriptionPlan({ gymId: '', name: '', price: '', durationInDays: '' });
      setSubscriptionPlans(await fetchSubscriptionPlans());
    } catch (err) {
      setCreateSubscriptionError(err.message);
    }
  };

  const handleSubscriptionSelection = (userId, planId) => {
    setSelectedSubscriptionPlans((current) => ({ ...current, [userId]: planId }));
  };

  const handleGymChange = (event) => {
    const { name, value } = event.target;
    setNewGym((current) => ({ ...current, [name]: value }));
  };

  const handleCreateGym = async (event) => {
    event.preventDefault();
    setGymError(null);
    setGymSuccess(null);

    try {
      await createGym({
        name: newGym.name,
        city: newGym.city
      });
      setGymSuccess('Gym created successfully.');
      setNewGym({ name: '', city: '' });
      setGyms(await fetchGyms());
    } catch (err) {
      setGymError(err.message);
    }
  };

  const handleAssignSubscription = async (user) => {
    setSubscriptionError(null);
    setSubscriptionSuccess(null);

    try {
      const selectedPlanId = selectedSubscriptionPlans[user.id];
      if (!selectedPlanId) {
        throw new Error('Please select a subscription plan for this user.');
      }
      await subscribeUserToPlan({ userId: user.id, subscriptionPlanId: selectedPlanId });
      setSubscriptionSuccess(`Subscription assigned to ${user.email}.`);
      setSelectedSubscriptionPlans((current) => ({ ...current, [user.id]: '' }));

      const activeSub = await fetchActiveUserSubscription(user.id);
      setActiveSubscriptions((current) => ({ ...current, [user.id]: activeSub }));
    } catch (err) {
      setSubscriptionError(err.message);
    }
  };

  return (
    <section className="page page-data">
      <h1>Super Admin Dashboard</h1>

      {!auth?.token ? (
        <p>Please login to view users.</p>
      ) : !canViewUsers ? (
        <p>You do not have permission to view this page.</p>
      ) : (
        <>
          {error && <p className="error">{error}</p>}
          {actionError && <p className="error">{actionError}</p>}
          {actionSuccess && <p className="success">{actionSuccess}</p>}

          <div className="dashboard-grid">
            <div className="full-width">
              <div className="data-table">
                <h2>Existing users</h2>
                <table>
                  <thead>
                    <tr>
                      <th>Email</th>
                      <th>Name</th>
                      <th>Role(s)</th>
                      <th>Active</th>
                      <th>Current subscription</th>
                      <th>Assign subscription</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {users?.map((user) => (
                      <tr key={user.id}>
                        <td>{user.email}</td>
                        <td>{user.firstName} {user.lastName}</td>
                        <td>{user.roles?.length ? user.roles.join(', ') : '-'}</td>
                        <td>{user.isActive ? 'Yes' : 'No'}</td>
                        <td>
                          {activeSubscriptions[user.id]
                            ? `${activeSubscriptions[user.id].subscriptionPlanName} (ends ${new Date(activeSubscriptions[user.id].endDate).toLocaleDateString()})`
                            : 'None'}
                        </td>
                        <td>
                          <div className="subscription-actions">
                            <select
                              value={selectedSubscriptionPlans[user.id] ?? ''}
                              onChange={(e) => handleSubscriptionSelection(user.id, e.target.value)}
                            >
                              <option value="">Select plan</option>
                              {subscriptionPlans?.map((plan) => (
                                <option key={plan.id} value={plan.id}>
                                  {plan.name} ({plan.price} / {plan.durationInDays}d)
                                </option>
                              ))}
                            </select>
                            <button type="button" onClick={() => handleAssignSubscription(user)} disabled={!subscriptionPlans?.length}>
                              Assign
                            </button>
                          </div>
                        </td>
                        <td>
                          <div className="inactive-user-actions">
                            <select
                              value={editRoles[user.id] ?? (user.roles?.[0] ?? 'User')}
                              onChange={(e) => handleRoleSelection(user.id, e.target.value)}
                            >
                              {roleOptions.map((option) => (
                                <option key={option.value} value={option.value}>{option.label}</option>
                              ))}
                            </select>
                            <button type="button" onClick={() => handleSaveRole(user)}>
                              Save role
                            </button>
                            <button type="button" onClick={() => handleToggleActive(user)}>
                              {user.isActive ? 'Deactivate' : 'Activate'}
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          {isSuperAdmin && (
            <section className="admin-section">
              <div className="admin-tabs">
                <button type="button" className={activeAdminTab === 'user' ? 'admin-tab active' : 'admin-tab'} onClick={() => setActiveAdminTab('user')}>
                  User Management
                </button>
                <button type="button" className={activeAdminTab === 'subscription' ? 'admin-tab active' : 'admin-tab'} onClick={() => setActiveAdminTab('subscription')}>
                  Subscription Management
                </button>
                <button type="button" className={activeAdminTab === 'gym' ? 'admin-tab active' : 'admin-tab'} onClick={() => setActiveAdminTab('gym')}>
                  Gym Management
                </button>
              </div>

              <div className="admin-panels">
                {activeAdminTab === 'user' && (
                  <div className="admin-panel">
                    <h2>User Management</h2>
                    <p className="admin-panel-description">Create new users and manage user roles directly from this panel.</p>
                    <form onSubmit={handleCreateUser} className="form-card">
                      <label>
                        First name
                        <input name="firstName" value={newUser.firstName} onChange={handleChange} required />
                      </label>
                      <label>
                        Last name
                        <input name="lastName" value={newUser.lastName} onChange={handleChange} required />
                      </label>
                      <label>
                        Date of birth
                        <input name="dateOfBirth" type="date" value={newUser.dateOfBirth} onChange={handleChange} required />
                      </label>
                      <label>
                        Email
                        <input name="email" type="email" value={newUser.email} onChange={handleChange} required />
                      </label>
                      <label>
                        Password
                        <input name="password" type="password" value={newUser.password} onChange={handleChange} required />
                      </label>
                      <label>
                        Role
                        <select name="role" value={newUser.role} onChange={handleChange}>
                          {roleOptions.map((option) => (
                            <option key={option.value} value={option.value}>{option.label}</option>
                          ))}
                        </select>
                      </label>
                      {createError && <p className="error">{createError}</p>}
                      {createSuccess && <p className="success">{createSuccess}</p>}
                      <button type="submit">Create user</button>
                    </form>
                  </div>
                )}

                {activeAdminTab === 'subscription' && (
                  <div className="admin-panel">
                    <h2>Subscription Management</h2>
                    <p className="admin-panel-description">Manage subscription plans and quickly create new offerings by gym.</p>
                    <form onSubmit={handleCreateSubscriptionPlan} className="form-card">
                      <label>
                        Gym
                        <select name="gymId" value={newSubscriptionPlan.gymId} onChange={handleSubscriptionChange} required>
                          <option value="">Select gym</option>
                          {gyms.map((gym) => (
                            <option key={gym.id} value={gym.id}>
                              {gym.name} ({gym.city})
                            </option>
                          ))}
                        </select>
                      </label>
                      <label>
                        Plan name
                        <input name="name" value={newSubscriptionPlan.name} onChange={handleSubscriptionChange} required />
                      </label>
                      <label>
                        Price
                        <input name="price" type="number" value={newSubscriptionPlan.price} onChange={handleSubscriptionChange} required />
                      </label>
                      <label>
                        Duration (days)
                        <input name="durationInDays" type="number" value={newSubscriptionPlan.durationInDays} onChange={handleSubscriptionChange} required />
                      </label>
                      {createSubscriptionError && <p className="error">{createSubscriptionError}</p>}
                      {createSubscriptionSuccess && <p className="success">{createSubscriptionSuccess}</p>}
                      <button type="submit" disabled={!gyms.length}>Create subscription plan</button>
                    </form>
                    {subscriptionPlans.length > 0 && (
                      <div className="data-table">
                        <h3>Available subscription plans</h3>
                        <table>
                          <thead>
                            <tr>
                              <th>Name</th>
                              <th>Price</th>
                              <th>Duration</th>
                            </tr>
                          </thead>
                          <tbody>
                            {subscriptionPlans.map((plan) => (
                              <tr key={plan.id}>
                                <td>{plan.name}</td>
                                <td>{plan.price}</td>
                                <td>{plan.durationInDays} days</td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>
                    )}
                  </div>
                )}

                {activeAdminTab === 'gym' && (
                  <div className="admin-panel">
                    <h2>Gym Management</h2>
                    <p className="admin-panel-description">Add new gyms and see the current locations in one place.</p>
                    <form onSubmit={handleCreateGym} className="form-card">
                      <label>
                        Gym name
                        <input name="name" value={newGym.name} onChange={handleGymChange} required />
                      </label>
                      <label>
                        City
                        <input name="city" value={newGym.city} onChange={handleGymChange} required />
                      </label>
                      {gymError && <p className="error">{gymError}</p>}
                      {gymSuccess && <p className="success">{gymSuccess}</p>}
                      <button type="submit">Create gym</button>
                    </form>
                    {gyms.length > 0 && (
                      <div className="data-table">
                        <h3>Existing gyms</h3>
                        <table>
                          <thead>
                            <tr>
                              <th>Name</th>
                              <th>City</th>
                            </tr>
                          </thead>
                          <tbody>
                            {gyms.map((gym) => (
                              <tr key={gym.id}>
                                <td>{gym.name}</td>
                                <td>{gym.city}</td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>
                    )}
                  </div>
                )}
              </div>
            </section>
          )}
        </>
      )}
    </section>
  );
}
