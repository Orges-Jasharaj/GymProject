import { useEffect, useState } from 'react';
import {
  addMealToNutritionPlan,
  fetchNutritionPlans
} from '../api';

const weekDays = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
const mealTypes = ['Breakfast', 'Lunch', 'Snack', 'Dinner'];

export default function MealsPage() {
  const [nutritionPlans, setNutritionPlans] = useState([]);
  const [mealForm, setMealForm] = useState({
    nutritionPlanId: '',
    dayOfWeek: 'Monday',
    mealType: 'Breakfast',
    name: '',
    description: '',
    calories: '',
    protein: '',
    carbohydrates: '',
    fats: ''
  });
  const [message, setMessage] = useState(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const plans = await fetchNutritionPlans();
      setNutritionPlans(plans);
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleMealChange = (event) => {
    const { name, value } = event.target;
    setMealForm((current) => ({ ...current, [name]: value }));
  };

  const handleAddMealToPlan = async (event) => {
    event.preventDefault();
    try {
      if (!mealForm.nutritionPlanId) {
        setMessage('Create or select a nutrition plan first.');
        return;
      }

      await addMealToNutritionPlan({
        nutritionPlanId: Number(mealForm.nutritionPlanId),
        dayOfWeek: mealForm.dayOfWeek,
        mealType: mealForm.mealType,
        name: mealForm.name,
        description: mealForm.description,
        calories: Number(mealForm.calories),
        protein: Number(mealForm.protein),
        carbohydrates: Number(mealForm.carbohydrates),
        fats: Number(mealForm.fats)
      });

      setMealForm((current) => ({
        ...current,
        name: '',
        description: '',
        calories: '',
        protein: '',
        carbohydrates: '',
        fats: ''
      }));

      setMessage('Meal added to nutrition plan.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  return (
    <section className="page page-data">
      <h1>Meals</h1>
      <div className="data-card">
        <h2>Add meal to plan</h2>
        <form className="entity-form" onSubmit={handleAddMealToPlan}>
          <label>
            Nutrition plan
            <select
              name="nutritionPlanId"
              value={mealForm.nutritionPlanId}
              onChange={handleMealChange}
              required
            >
              <option value="">Select plan</option>
              {nutritionPlans.map((plan) => (
                <option key={plan.id} value={plan.id}>
                  {plan.name}
                </option>
              ))}
            </select>
          </label>
          <label>
            Day of week
            <select name="dayOfWeek" value={mealForm.dayOfWeek} onChange={handleMealChange} required>
              {weekDays.map((day) => (
                <option key={day} value={day}>
                  {day}
                </option>
              ))}
            </select>
          </label>
          <label>
            Meal type
            <select name="mealType" value={mealForm.mealType} onChange={handleMealChange} required>
              {mealTypes.map((mealType) => (
                <option key={mealType} value={mealType}>
                  {mealType}
                </option>
              ))}
            </select>
          </label>
          <label>
            Meal name
            <input name="name" value={mealForm.name} onChange={handleMealChange} required />
          </label>
          <label>
            Description
            <textarea name="description" value={mealForm.description} onChange={handleMealChange} rows="3" required />
          </label>
          <label>
            Calories
            <input name="calories" type="number" value={mealForm.calories} onChange={handleMealChange} required />
          </label>
          <label>
            Protein
            <input name="protein" type="number" value={mealForm.protein} onChange={handleMealChange} required />
          </label>
          <label>
            Carbohydrates
            <input name="carbohydrates" type="number" value={mealForm.carbohydrates} onChange={handleMealChange} required />
          </label>
          <label>
            Fats
            <input name="fats" type="number" value={mealForm.fats} onChange={handleMealChange} required />
          </label>
          <button type="submit">Add meal</button>
        </form>
      </div>
      {message && <p className="form-message">{message}</p>}
    </section>
  );
}
