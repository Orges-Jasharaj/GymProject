import { useEffect, useState } from 'react';
import { createFitnessPlan, createPlanExercise, fetchExercises, fetchFitnessPlans } from '../api';

const emptyPlanItem = { exerciseId: '', sets: 3, reps: 10, order: 1 };

export default function FitnessPlansPage() {
  const [plans, setPlans] = useState([]);
  const [exercises, setExercises] = useState([]);
  const [form, setForm] = useState({ name: '', description: '' });
  const [planItems, setPlanItems] = useState([{ ...emptyPlanItem }]);
  const [message, setMessage] = useState(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [plansResult, exercisesResult] = await Promise.all([fetchFitnessPlans(), fetchExercises()]);
      setPlans(plansResult);
      setExercises(exercisesResult);
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  };

  const handleItemChange = (index, field, value) => {
    setPlanItems((current) =>
      current.map((item, idx) =>
        idx === index ? { ...item, [field]: field === 'exerciseId' ? value : Number(value) } : item
      )
    );
  };

  const addPlanItem = () => {
    setPlanItems((current) => [...current, { ...emptyPlanItem, order: current.length + 1 }]);
  };

  const removePlanItem = (index) => {
    setPlanItems((current) => current.filter((_, idx) => idx !== index));
  };

  const handleCreate = async (event) => {
    event.preventDefault();

    if (!planItems.some((item) => item.exerciseId)) {
      setMessage('Add at least one exercise to the plan.');
      return;
    }

    try {
      setMessage(null);
      const createdPlan = await createFitnessPlan(form);
      const planId = createdPlan.id;

      await Promise.all(
        planItems
          .filter((item) => item.exerciseId)
          .map((item, index) =>
            createPlanExercise({
              fitnessPlanId: planId,
              exerciseId: item.exerciseId,
              sets: Number(item.sets),
              reps: Number(item.reps),
              exerciseOrder: Number(item.order ?? index + 1)
            })
          )
      );

      setForm({ name: '', description: '' });
      setPlanItems([{ ...emptyPlanItem }]);
      await loadData();
      setMessage('Fitness plan created with exercises.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  return (
    <section className="page page-data">
      <h1>Fitness plans</h1>
      <div className="data-card">
        <h2>Add fitness plan</h2>
        <form className="entity-form" onSubmit={handleCreate}>
          <label>
            Name
            <input name="name" value={form.name} onChange={handleChange} required />
          </label>
          <label>
            Description
            <textarea name="description" value={form.description} onChange={handleChange} rows="3" />
          </label>
          <div className="plan-exercise-list">
            <h3>Exercises in this plan</h3>
            {planItems.map((item, index) => (
              <div key={index} className="plan-exercise-row">
                <label>
                  Exercise
                  <select
                    value={item.exerciseId}
                    onChange={(event) => handleItemChange(index, 'exerciseId', event.target.value)}
                    required
                  >
                    <option value="">Select exercise</option>
                    {exercises.map((exercise) => (
                      <option key={exercise.id} value={exercise.id}>
                        {exercise.name}
                      </option>
                    ))}
                  </select>
                </label>
                <label>
                  Sets
                  <input
                    type="number"
                    min="1"
                    name="sets"
                    value={item.sets}
                    onChange={(event) => handleItemChange(index, 'sets', event.target.value)}
                    required
                  />
                </label>
                <label>
                  Reps
                  <input
                    type="number"
                    min="1"
                    name="reps"
                    value={item.reps}
                    onChange={(event) => handleItemChange(index, 'reps', event.target.value)}
                    required
                  />
                </label>
                <label>
                  Order
                  <input
                    type="number"
                    min="1"
                    name="order"
                    value={item.order}
                    onChange={(event) => handleItemChange(index, 'order', event.target.value)}
                    required
                  />
                </label>
                <button type="button" onClick={() => removePlanItem(index)}>
                  Remove
                </button>
              </div>
            ))}
            <button type="button" onClick={addPlanItem}>
              Add exercise to plan
            </button>
          </div>
          <button type="submit">Create plan</button>
        </form>
      </div>
      {message && <p className="form-message">{message}</p>}
      <div className="data-table">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Description</th>
            </tr>
          </thead>
          <tbody>
            {plans?.map((plan) => (
              <tr key={plan.id}>
                <td>{plan.name}</td>
                <td>{plan.description}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
