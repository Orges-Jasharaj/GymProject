import { useEffect, useState } from 'react';
import { createExercise, fetchExercises, fetchExercisesByMuscleGroup } from '../api';
import { useAuth } from '../hooks/useAuth';

export default function ExercisesPage() {
  const { auth } = useAuth();
  const [exercises, setExercises] = useState([]);
  const [form, setForm] = useState({ name: '', description: '', muscleGroup: '', equipment: '' });
  const [searchGroup, setSearchGroup] = useState('');
  const [message, setMessage] = useState(null);

  const isAdmin = auth?.roles?.some((role) => role === 'SuperAdmin' || role === 'Admin');

  useEffect(() => {
    fetchExercises().then(setExercises).catch((error) => setMessage(error.message));
  }, []);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  };

  const handleCreate = async (event) => {
    event.preventDefault();
    try {
      const newExercise = await createExercise(form);
      setExercises((current) => [...current, newExercise]);
      setForm({ name: '', description: '', muscleGroup: '', equipment: '' });
      setMessage('Exercise created.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleSearchChange = (event) => {
    setSearchGroup(event.target.value);
  };

  const handleSearch = async () => {
    setMessage(null);
    try {
      if (!searchGroup.trim()) {
        const allExercises = await fetchExercises();
        setExercises(allExercises);
        return;
      }
      const filtered = await fetchExercisesByMuscleGroup(searchGroup.trim());
      setExercises(filtered);
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleClearSearch = async () => {
    setSearchGroup('');
    setMessage(null);
    try {
      const allExercises = await fetchExercises();
      setExercises(allExercises);
    } catch (error) {
      setMessage(error.message);
    }
  };

  return (
    <section className="page page-data">
      <h1>Exercises</h1>
      {isAdmin && (
        <div className="data-card">
          <h2>Add exercise</h2>
          <form className="entity-form" onSubmit={handleCreate}>
            <label>
              Name
              <input name="name" value={form.name} onChange={handleChange} required />
            </label>
            <label>
              Description
              <input name="description" value={form.description} onChange={handleChange} required />
            </label>
            <label>
              Muscle group
              <input name="muscleGroup" value={form.muscleGroup} onChange={handleChange} />
            </label>
            <label>
              Equipment
              <input name="equipment" value={form.equipment} onChange={handleChange} />
            </label>
            <button type="submit">Create exercise</button>
          </form>
        </div>
      )}
      <div className="data-card">
        <h2>Search exercises by muscle group</h2>
        <div className="entity-form">
          <label>
            Muscle group
            <input
              name="searchGroup"
              value={searchGroup}
              onChange={handleSearchChange}
              placeholder="Chest, Back, Legs..."
            />
          </label>
          <div className="button-row">
            <button type="button" onClick={handleSearch}>Search</button>
            <button type="button" onClick={handleClearSearch}>Clear</button>
          </div>
        </div>
      </div>
      {message && <p className="form-message">{message}</p>}
      <div className="data-table">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Muscle</th>
              <th>Equipment</th>
            </tr>
          </thead>
          <tbody>
            {exercises?.map((exercise) => (
              <tr key={exercise.id}>
                <td>{exercise.name}</td>
                <td>{exercise.muscleGroup}</td>
                <td>{exercise.equipment}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
