import { useEffect, useState } from 'react';
import {
  createNutritionPlan,
  deleteNutritionPlan,
  deletePlanMeal,
  fetchNutritionPlanDetails,
  fetchNutritionPlans,
  updateNutritionPlan,
  updatePlanMeal
} from '../api';

const weekDays = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
const mealTypes = ['Breakfast', 'Lunch', 'Snack', 'Dinner'];

export default function ExistingMealPlansPage() {
  const [plans, setPlans] = useState([]);
  const [selectedPlanDetails, setSelectedPlanDetails] = useState(null);
  const [planForm, setPlanForm] = useState({ name: '' });
  const [editingPlanId, setEditingPlanId] = useState(null);
  const [editingPlanName, setEditingPlanName] = useState('');
  const [editingMeal, setEditingMeal] = useState(null);
  const [message, setMessage] = useState(null);

  useEffect(() => {
    loadPlans();
  }, []);

  const loadPlans = async () => {
    try {
      const data = await fetchNutritionPlans();
      setPlans(data);
      setMessage(null);
    } catch (error) {
      setMessage(error.message);
    }
  };

  const loadPlanDetails = async (planId) => {
    try {
      const details = await fetchNutritionPlanDetails(planId);
      setSelectedPlanDetails(details);
      setMessage(null);
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleCreatePlan = async (event) => {
    event.preventDefault();
    try {
      await createNutritionPlan(planForm);
      setPlanForm({ name: '' });
      await loadPlans();
      setMessage('Meal plan created.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleDeletePlan = async (plan) => {
    if (!window.confirm(`Delete meal plan "${plan.name}"?`)) {
      return;
    }

    try {
      await deleteNutritionPlan(plan.id);
      if (selectedPlanDetails?.id === plan.id) {
        setSelectedPlanDetails(null);
      }
      setEditingPlanId(null);
      await loadPlans();
      setMessage('Meal plan deleted.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  const startEditPlan = (plan) => {
    setEditingPlanId(plan.id);
    setEditingPlanName(plan.name);
  };

  const savePlanUpdate = async (planId) => {
    try {
      await updateNutritionPlan(planId, { name: editingPlanName });
      setEditingPlanId(null);
      await loadPlans();
      if (selectedPlanDetails?.id === planId) {
        await loadPlanDetails(planId);
      }
      setMessage('Meal plan updated.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  const startEditMeal = (meal) => {
    setEditingMeal({
      planMealId: meal.planMealId,
      dayOfWeek: meal.dayOfWeek,
      mealType: meal.mealType,
      name: meal.name,
      description: meal.description,
      calories: meal.calories,
      protein: meal.protein,
      carbohydrates: meal.carbohydrates,
      fats: meal.fats
    });
  };

  const saveMealUpdate = async () => {
    try {
      await updatePlanMeal(editingMeal.planMealId, {
        dayOfWeek: editingMeal.dayOfWeek,
        mealType: editingMeal.mealType,
        name: editingMeal.name,
        description: editingMeal.description,
        calories: Number(editingMeal.calories),
        protein: Number(editingMeal.protein),
        carbohydrates: Number(editingMeal.carbohydrates),
        fats: Number(editingMeal.fats)
      });
      setEditingMeal(null);
      if (selectedPlanDetails?.id) {
        await loadPlanDetails(selectedPlanDetails.id);
      }
      setMessage('Meal updated.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleDeleteMeal = async (planMealId) => {
    if (!window.confirm('Delete this meal from plan?')) {
      return;
    }

    try {
      await deletePlanMeal(planMealId);
      if (selectedPlanDetails?.id) {
        await loadPlanDetails(selectedPlanDetails.id);
      }
      setMessage('Meal deleted from plan.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  return (
    <section className="page page-data">
      <h1>Existing meal plans</h1>
      <div className="data-card">
        <h2>Create meal plan</h2>
        <form className="entity-form" onSubmit={handleCreatePlan}>
          <label>
            Plan name
            <input
              name="name"
              value={planForm.name}
              onChange={(event) => setPlanForm({ name: event.target.value })}
              required
            />
          </label>
          <button type="submit">Create plan</button>
        </form>
      </div>
      {message && <p className="form-message">{message}</p>}
      <div className="data-card">
        <h2>Plans</h2>
        <div className="data-table">
          <table>
            <thead>
              <tr>
                <th>Plan name</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {plans.map((plan) => (
                <tr key={plan.id}>
                  <td>{plan.name}</td>
                  <td>
                    <button type="button" onClick={() => loadPlanDetails(plan.id)}>View details</button>
                    <button type="button" onClick={() => startEditPlan(plan)}>Update</button>
                    <button type="button" className="danger-button" onClick={() => handleDeletePlan(plan)}>Delete</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
      {editingPlanId && (
        <div className="data-card">
          <h2>Update meal plan</h2>
          <form
            className="entity-form"
            onSubmit={(event) => {
              event.preventDefault();
              savePlanUpdate(editingPlanId);
            }}
          >
            <label>
              Plan name
              <input value={editingPlanName} onChange={(event) => setEditingPlanName(event.target.value)} required />
            </label>
            <div className="form-footer">
              <button type="submit">Save</button>
              <button type="button" className="secondary-button" onClick={() => setEditingPlanId(null)}>Cancel</button>
            </div>
          </form>
        </div>
      )}
      {selectedPlanDetails && (
        <div className="data-card">
          <h2>Plan details: {selectedPlanDetails.name}</h2>
          <div className="data-table">
            <table>
              <thead>
                <tr>
                  <th>Day</th>
                  <th>Meal type</th>
                  <th>Name</th>
                  <th>Description</th>
                  <th>Calories</th>
                  <th>Protein</th>
                  <th>Carbs</th>
                  <th>Fats</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {selectedPlanDetails.meals?.map((meal) => (
                  <tr key={meal.planMealId}>
                    <td>{meal.dayOfWeek}</td>
                    <td>{meal.mealType}</td>
                    <td>{meal.name}</td>
                    <td>{meal.description}</td>
                    <td>{meal.calories}</td>
                    <td>{meal.protein}</td>
                    <td>{meal.carbohydrates}</td>
                    <td>{meal.fats}</td>
                    <td>
                      <button type="button" onClick={() => startEditMeal(meal)}>Update</button>
                      <button type="button" className="danger-button" onClick={() => handleDeleteMeal(meal.planMealId)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
      {editingMeal && (
        <div className="data-card">
          <h2>Update meal</h2>
          <form
            className="entity-form"
            onSubmit={(event) => {
              event.preventDefault();
              saveMealUpdate();
            }}
          >
            <label>
              Day of week
              <select
                value={editingMeal.dayOfWeek}
                onChange={(event) => setEditingMeal((current) => ({ ...current, dayOfWeek: event.target.value }))}
                required
              >
                {weekDays.map((day) => (
                  <option key={day} value={day}>{day}</option>
                ))}
              </select>
            </label>
            <label>
              Meal type
              <select
                value={editingMeal.mealType}
                onChange={(event) => setEditingMeal((current) => ({ ...current, mealType: event.target.value }))}
                required
              >
                {mealTypes.map((type) => (
                  <option key={type} value={type}>{type}</option>
                ))}
              </select>
            </label>
            <label>
              Name
              <input
                value={editingMeal.name}
                onChange={(event) => setEditingMeal((current) => ({ ...current, name: event.target.value }))}
                required
              />
            </label>
            <label>
              Description
              <textarea
                value={editingMeal.description}
                onChange={(event) => setEditingMeal((current) => ({ ...current, description: event.target.value }))}
                rows="3"
                required
              />
            </label>
            <label>
              Calories
              <input
                type="number"
                value={editingMeal.calories}
                onChange={(event) => setEditingMeal((current) => ({ ...current, calories: event.target.value }))}
                required
              />
            </label>
            <label>
              Protein
              <input
                type="number"
                value={editingMeal.protein}
                onChange={(event) => setEditingMeal((current) => ({ ...current, protein: event.target.value }))}
                required
              />
            </label>
            <label>
              Carbohydrates
              <input
                type="number"
                value={editingMeal.carbohydrates}
                onChange={(event) => setEditingMeal((current) => ({ ...current, carbohydrates: event.target.value }))}
                required
              />
            </label>
            <label>
              Fats
              <input
                type="number"
                value={editingMeal.fats}
                onChange={(event) => setEditingMeal((current) => ({ ...current, fats: event.target.value }))}
                required
              />
            </label>
            <div className="form-footer">
              <button type="submit">Save</button>
              <button type="button" className="secondary-button" onClick={() => setEditingMeal(null)}>Cancel</button>
            </div>
          </form>
        </div>
      )}
    </section>
  );
}
