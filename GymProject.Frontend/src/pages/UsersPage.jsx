import { useEffect, useState } from 'react';
import { fetchUsers, createUserWithRole, reactivateUser, deactivateUser, updateUserRole } from '../api';
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

  const isSuperAdmin = auth?.roles?.includes('SuperAdmin');
  const canViewUsers = isSuperAdmin;

  useEffect(() => {
    setError(null);
    if (!auth?.token || !isSuperAdmin) return;

    fetchUsers()
      .then(setUsers)
      .catch((err) => setError(err.message));
  }, [auth, canViewUsers]);

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
            <div className="data-table">
              <h2>Existing users</h2>
              <table>
                <thead>
                  <tr>
                    <th>Email</th>
                    <th>Name</th>
                    <th>Role(s)</th>
                    <th>Active</th>
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

            {isSuperAdmin && (
              <div className="admin-panel">
                <h2>Create new user</h2>
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
          </div>
        </>
      )}
    </section>
  );
}
