import { useEffect, useState } from 'react';
import { createMeal, fetchMeals } from '../api';

export default function MealsPage() {
  const [meals, setMeals] = useState([]);
  const [form, setForm] = useState({ name: '', description: '', calories: '', protein: '', carbohydrates: '', fats: '' });
  const [message, setMessage] = useState(null);

  useEffect(() => {
    fetchMeals().then(setMeals).catch((error) => setMessage(error.message));
  }, []);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  };

  const handleCreate = async (event) => {
    event.preventDefault();
    try {
      const created = await createMeal({
        name: form.name,
        description: form.description,
        calories: Number(form.calories),
        protein: Number(form.protein),
        carbohydrates: Number(form.carbohydrates),
        fats: Number(form.fats)
      });
      setMeals((current) => [...current, created]);
      setForm({ name: '', description: '', calories: '', protein: '', carbohydrates: '', fats: '' });
      setMessage('Meal created.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  return (
    <section className="page page-data">
      <h1>Meals</h1>
      <div className="data-card">
        <h2>Add meal</h2>
        <form className="entity-form" onSubmit={handleCreate}>
          <label>
            Name
            <input name="name" value={form.name} onChange={handleChange} required />
          </label>
          <label>
            Description
            <textarea name="description" value={form.description} onChange={handleChange} rows="3" required />
          </label>
          <label>
            Calories
            <input name="calories" type="number" value={form.calories} onChange={handleChange} required />
          </label>
          <label>
            Protein
            <input name="protein" type="number" value={form.protein} onChange={handleChange} required />
          </label>
          <label>
            Carbohydrates
            <input name="carbohydrates" type="number" value={form.carbohydrates} onChange={handleChange} required />
          </label>
          <label>
            Fats
            <input name="fats" type="number" value={form.fats} onChange={handleChange} required />
          </label>
          <button type="submit">Create meal</button>
        </form>
      </div>
      {message && <p className="form-message">{message}</p>}
      <div className="data-table">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Calories</th>
              <th>Protein</th>
              <th>Carbs</th>
              <th>Fats</th>
            </tr>
          </thead>
          <tbody>
            {meals?.map((meal) => (
              <tr key={meal.id}>
                <td>{meal.name}</td>
                <td>{meal.calories}</td>
                <td>{meal.protein}</td>
                <td>{meal.carbohydrates}</td>
                <td>{meal.fats}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
