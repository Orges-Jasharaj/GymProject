import { useEffect, useState } from 'react';
import { fetchUsers } from '../api';
import { useAuth } from '../hooks/useAuth';

export default function UsersPage() {
  const { auth } = useAuth();
  const [users, setUsers] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!auth?.token) return;
    fetchUsers().then(setUsers).catch((err) => setError(err.message));
  }, [auth]);

  return (
    <section className="page page-data">
      <h1>User management</h1>
      {!auth?.token ? (
        <p>Please login to view users.</p>
      ) : error ? (
        <p className="error">{error}</p>
      ) : (
        <div className="data-table">
          <table>
            <thead>
              <tr>
                <th>Email</th>
                <th>Name</th>
                <th>Active</th>
              </tr>
            </thead>
            <tbody>
              {users?.map((user) => (
                <tr key={user.id}>
                  <td>{user.email}</td>
                  <td>{user.firstName} {user.lastName}</td>
                  <td>{user.isActive ? 'Yes' : 'No'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
