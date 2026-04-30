import { useEffect, useState } from 'react';
import { createFitnessPlan, createPlanExercise, fetchExercises } from '../api';

const emptyExercise = { exerciseId: '', sets: 3, reps: 10, order: 1 };
const emptyPlanGroup = { dayOfWeek: '', focus: '', exercises: [{ ...emptyExercise }] };
const weekDays = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
export default function FitnessPlansPage() {
  const [exercises, setExercises] = useState([]);
  const [form, setForm] = useState({ name: '', description: '' });
  const [planGroups, setPlanGroups] = useState([{ ...emptyPlanGroup }]);
  const [message, setMessage] = useState(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const exercisesResult = await fetchExercises();
      setExercises(exercisesResult);
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  };

  const handleGroupChange = (groupIndex, field, value) => {
    setPlanGroups((current) =>
      current.map((group, idx) =>
        idx === groupIndex ? { ...group, [field]: value } : group
      )
    );
  };

  const handleExerciseChange = (groupIndex, exerciseIndex, field, value) => {
    setPlanGroups((current) =>
      current.map((group, idx) =>
        idx !== groupIndex
          ? group
          : {
              ...group,
              exercises: group.exercises.map((exercise, eIdx) =>
                eIdx === exerciseIndex
                  ? {
                      ...exercise,
                      [field]: field === 'exerciseId' ? value : Number(value)
                    }
                  : exercise
              )
            }
      )
    );
  };

  const addPlanGroup = () => {
    setPlanGroups((current) => [...current, { ...emptyPlanGroup }]);
  };

  const removePlanGroup = (groupIndex) => {
    setPlanGroups((current) => current.filter((_, idx) => idx !== groupIndex));
  };

  const addExerciseToGroup = (groupIndex) => {
    setPlanGroups((current) =>
      current.map((group, idx) =>
        idx !== groupIndex
          ? group
          : {
              ...group,
              exercises: [
                ...group.exercises,
                { ...emptyExercise, order: group.exercises.length + 1 }
              ]
            }
      )
    );
  };

  const removeExerciseFromGroup = (groupIndex, exerciseIndex) => {
    setPlanGroups((current) =>
      current.map((group, idx) =>
        idx !== groupIndex
          ? group
          : {
              ...group,
              exercises: group.exercises.filter((_, eIdx) => eIdx !== exerciseIndex)
            }
      )
    );
  };

  const handleCreate = async (event) => {
    event.preventDefault();

    if (!planGroups.some((group) => group.exercises.some((item) => item.exerciseId))) {
      setMessage('Add at least one exercise to the plan.');
      return;
    }

    try {
      setMessage(null);
      const createdPlan = await createFitnessPlan(form);
      const planId = createdPlan.id;

      const createRequests = planGroups
        .filter((group) => group.dayOfWeek && group.exercises.some((exercise) => exercise.exerciseId))
        .flatMap((group) =>
          group.exercises
            .filter((exercise) => exercise.exerciseId)
            .map((exercise, index) =>
              createPlanExercise({
                fitnessPlanId: planId,
                exerciseId: exercise.exerciseId,
                sets: Number(exercise.sets),
                reps: Number(exercise.reps),
                exerciseOrder: Number(exercise.order ?? index + 1),
                dayOfWeek: group.dayOfWeek,
                focus: group.focus
              })
            )
        );

      if (!createRequests.length) {
        setMessage('Add at least one exercise to a day group.');
        return;
      }

      await Promise.all(createRequests);

      setForm({ name: '', description: '' });
      setPlanGroups([{ ...emptyPlanGroup }]);
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
            <h3>Day groups</h3>
            {planGroups.map((group, groupIndex) => (
              <div key={groupIndex} className="plan-exercise-group">
                <div className="plan-group-header">
                  <label>
                    Day of week
                    <select
                      value={group.dayOfWeek}
                      onChange={(event) => handleGroupChange(groupIndex, 'dayOfWeek', event.target.value)}
                      required
                    >
                      <option value="">Choose day</option>
                      {weekDays.map((day) => (
                        <option key={day} value={day}>
                          {day}
                        </option>
                      ))}
                    </select>
                  </label>
                  <label>
                    Focus
                    <input
                      type="text"
                      value={group.focus}
                      onChange={(event) => handleGroupChange(groupIndex, 'focus', event.target.value)}
                      placeholder="Chest and Triceps"
                    />
                  </label>
                  <button type="button" onClick={() => removePlanGroup(groupIndex)}>
                    Remove day
                  </button>
                </div>
                {group.exercises.map((exercise, exerciseIndex) => (
                  <div key={exerciseIndex} className="plan-exercise-row">
                    <label>
                      Exercise
                      <select
                        value={exercise.exerciseId}
                        onChange={(event) => handleExerciseChange(groupIndex, exerciseIndex, 'exerciseId', event.target.value)}
                        required
                      >
                        <option value="">Select exercise</option>
                        {exercises.map((exerciseItem) => (
                          <option key={exerciseItem.id} value={exerciseItem.id}>
                            {exerciseItem.name}
                          </option>
                        ))}
                      </select>
                    </label>
                    <label>
                      Sets
                      <input
                        type="number"
                        min="1"
                        value={exercise.sets}
                        onChange={(event) => handleExerciseChange(groupIndex, exerciseIndex, 'sets', event.target.value)}
                        required
                      />
                    </label>
                    <label>
                      Reps
                      <input
                        type="number"
                        min="1"
                        value={exercise.reps}
                        onChange={(event) => handleExerciseChange(groupIndex, exerciseIndex, 'reps', event.target.value)}
                        required
                      />
                    </label>
                    <label>
                      Order
                      <input
                        type="number"
                        min="1"
                        value={exercise.order}
                        onChange={(event) => handleExerciseChange(groupIndex, exerciseIndex, 'order', event.target.value)}
                        required
                      />
                    </label>
                    <button type="button" onClick={() => removeExerciseFromGroup(groupIndex, exerciseIndex)}>
                      Remove exercise
                    </button>
                  </div>
                ))}
                <button type="button" onClick={() => addExerciseToGroup(groupIndex)}>
                  Add exercise to this day
                </button>
              </div>
            ))}
            <button type="button" onClick={addPlanGroup}>
              Add day group
            </button>
          </div>
          <button type="submit">Create plan</button>
        </form>
      </div>
      {message && <p className="form-message">{message}</p>}
    </section>
  );
}
