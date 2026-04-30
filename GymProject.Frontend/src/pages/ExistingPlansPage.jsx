import { useEffect, useMemo, useState } from 'react';
import {
  createPlanExercise,
  deleteFitnessPlan,
  deletePlanExercise,
  fetchExercises,
  fetchFitnessPlanDetails,
  fetchFitnessPlans,
  fetchPlanExercises,
  updateFitnessPlan
} from '../api';

const weekDays = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
const emptyExercise = { exerciseId: '', sets: 3, reps: 10, order: 1 };
const emptyPlanGroup = { dayOfWeek: '', focus: '', exercises: [{ ...emptyExercise }] };

export default function ExistingPlansPage() {
  const [plans, setPlans] = useState([]);
  const [exercises, setExercises] = useState([]);
  const [allPlanExercises, setAllPlanExercises] = useState([]);
  const [selectedPlanDetails, setSelectedPlanDetails] = useState(null);
  const [editingPlanId, setEditingPlanId] = useState(null);
  const [editForm, setEditForm] = useState({ name: '', description: '' });
  const [editPlanGroups, setEditPlanGroups] = useState([{ ...emptyPlanGroup }]);
  const [message, setMessage] = useState(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [plansResult, exercisesResult, planExercisesResult] = await Promise.all([
        fetchFitnessPlans(),
        fetchExercises(),
        fetchPlanExercises()
      ]);
      setPlans(plansResult);
      setExercises(exercisesResult);
      setAllPlanExercises(planExercisesResult);
      setMessage(null);
    } catch (error) {
      setMessage(error.message);
    }
  };

  const loadPlanDetails = async (planId) => {
    try {
      const details = await fetchFitnessPlanDetails(planId);
      setSelectedPlanDetails(details);
      setMessage(null);
    } catch (error) {
      setMessage(error.message);
    }
  };

  const startEditPlan = (plan) => {
    const applyEditStateFromDetails = async () => {
      const details = selectedPlanDetails?.id === plan.id
        ? selectedPlanDetails
        : await fetchFitnessPlanDetails(plan.id);

      const groupsMap = details?.exercises?.reduce((groups, exercise) => {
        const key = `${exercise.dayOfWeek || 'Ungrouped'}::${exercise.focus || ''}`;
        if (!groups[key]) {
          groups[key] = {
            dayOfWeek: exercise.dayOfWeek || '',
            focus: exercise.focus || '',
            exercises: []
          };
        }
        groups[key].exercises.push({
          exerciseId: exercise.exerciseId,
          sets: exercise.sets,
          reps: exercise.reps,
          order: exercise.order
        });
        return groups;
      }, {});

      const mappedGroups = groupsMap
        ? Object.values(groupsMap).map((group) => ({
            ...group,
            exercises: group.exercises
              .slice()
              .sort((a, b) => a.order - b.order)
          }))
        : [{ ...emptyPlanGroup }];

      setEditingPlanId(plan.id);
      setEditForm({
        name: plan.name || '',
        description: plan.description || ''
      });
      setEditPlanGroups(mappedGroups.length ? mappedGroups : [{ ...emptyPlanGroup }]);
      setMessage(null);
    };

    applyEditStateFromDetails().catch((error) => {
      setMessage(error.message);
    });
  };

  const cancelEditPlan = () => {
    setEditingPlanId(null);
    setEditForm({ name: '', description: '' });
  };

  const handleEditFieldChange = (event) => {
    const { name, value } = event.target;
    setEditForm((current) => ({ ...current, [name]: value }));
  };

  const handleGroupChange = (groupIndex, field, value) => {
    setEditPlanGroups((current) =>
      current.map((group, idx) =>
        idx === groupIndex ? { ...group, [field]: value } : group
      )
    );
  };

  const handleExerciseChange = (groupIndex, exerciseIndex, field, value) => {
    setEditPlanGroups((current) =>
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
    setEditPlanGroups((current) => [...current, { ...emptyPlanGroup }]);
  };

  const removePlanGroup = (groupIndex) => {
    setEditPlanGroups((current) => current.filter((_, idx) => idx !== groupIndex));
  };

  const addExerciseToGroup = (groupIndex) => {
    setEditPlanGroups((current) =>
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
    setEditPlanGroups((current) =>
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

  const savePlanUpdate = async (planId) => {
    try {
      if (!editPlanGroups.some((group) => group.exercises.some((item) => item.exerciseId))) {
        setMessage('Add at least one exercise to the plan.');
        return;
      }

      await updateFitnessPlan(planId, {
        name: editForm.name,
        description: editForm.description
      });

      const existingPlanExercises = allPlanExercises.filter((item) => item.fitnessPlanId === planId);
      await Promise.all(existingPlanExercises.map((item) => deletePlanExercise(item.id)));

      const createRequests = editPlanGroups
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

      await loadData();
      if (selectedPlanDetails?.id === planId) {
        await loadPlanDetails(planId);
      }

      cancelEditPlan();
      setMessage('Plan updated successfully.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleDeletePlan = async (plan) => {
    const isConfirmed = window.confirm(`Delete plan "${plan.name}"?`);
    if (!isConfirmed) {
      return;
    }

    try {
      await deleteFitnessPlan(plan.id);
      if (selectedPlanDetails?.id === plan.id) {
        setSelectedPlanDetails(null);
      }
      if (editingPlanId === plan.id) {
        cancelEditPlan();
      }
      await loadData();
      setMessage('Plan deleted successfully.');
    } catch (error) {
      setMessage(error.message);
    }
  };

  const sortedPlanDayGroups = useMemo(() => {
    const planDayGroups = selectedPlanDetails?.exercises?.reduce((groups, exercise) => {
      const day = exercise.dayOfWeek || 'Ungrouped';
      if (!groups[day]) {
        groups[day] = { day, focus: exercise.focus, exercises: [] };
      }
      groups[day].exercises.push(exercise);
      return groups;
    }, {});

    return planDayGroups
      ? Object.values(planDayGroups).sort((a, b) =>
          (weekDays.indexOf(a.day) || 0) - (weekDays.indexOf(b.day) || 0)
        )
      : [];
  }, [selectedPlanDetails]);

  return (
    <section className="page page-data">
      <h1>Existing plans</h1>
      {message && <p className="form-message">{message}</p>}
      <div className="data-card">
        <h2>Existing fitness plans</h2>
        <div className="data-table">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Description</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {plans?.map((plan) => (
                <tr key={plan.id}>
                  <td>{plan.name}</td>
                  <td>{plan.description}</td>
                  <td>
                    <button type="button" onClick={() => loadPlanDetails(plan.id)}>
                      View details
                    </button>
                    <button type="button" onClick={() => startEditPlan(plan)}>
                      Update
                    </button>
                    <button type="button" className="danger-button" onClick={() => handleDeletePlan(plan)}>
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
      {editingPlanId && (
        <div className="data-card">
          <h2>Update fitness plan</h2>
          <form
            className="entity-form"
            onSubmit={(event) => {
              event.preventDefault();
              savePlanUpdate(editingPlanId);
            }}
          >
            <label>
              Name
              <input
                name="name"
                value={editForm.name}
                onChange={handleEditFieldChange}
                required
              />
            </label>
            <label>
              Description
              <textarea
                name="description"
                value={editForm.description}
                onChange={handleEditFieldChange}
                rows="3"
              />
            </label>
            <div className="plan-exercise-list">
              <h3>Day groups</h3>
              {editPlanGroups.map((group, groupIndex) => (
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
            <div className="form-footer">
              <button type="submit">Save</button>
              <button type="button" className="secondary-button" onClick={cancelEditPlan}>
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}
      {selectedPlanDetails && (
        <div className="data-card">
          <h2>Plan details: {selectedPlanDetails.name}</h2>
          <p>{selectedPlanDetails.description}</p>
          {sortedPlanDayGroups.length ? (
            sortedPlanDayGroups.map((group) => (
              <div key={group.day} className="plan-day-group">
                <h3>{group.day}</h3>
                {group.focus && <p><strong>Focus:</strong> {group.focus}</p>}
                <div className="data-table">
                  <table>
                    <thead>
                      <tr>
                        <th>Exercise</th>
                        <th>Sets</th>
                        <th>Reps</th>
                        <th>Order</th>
                      </tr>
                    </thead>
                    <tbody>
                      {group.exercises
                        .slice()
                        .sort((a, b) => a.order - b.order)
                        .map((exercise, index) => (
                          <tr key={`${exercise.exerciseId}-${index}`}>
                            <td>{exercise.name}</td>
                            <td>{exercise.sets}</td>
                            <td>{exercise.reps}</td>
                            <td>{exercise.order}</td>
                          </tr>
                        ))}
                    </tbody>
                  </table>
                </div>
              </div>
            ))
          ) : (
            <p>No exercises have been added to this plan yet.</p>
          )}
        </div>
      )}
    </section>
  );
}
