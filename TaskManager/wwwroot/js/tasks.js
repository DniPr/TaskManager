document.addEventListener("DOMContentLoaded", () => {
    const statusSelects =
        document.querySelectorAll(".task-status-select");

    statusSelects.forEach(select => {

        select.dataset.previousValue = select.value;

        select.addEventListener("change", async function () {

            const taskId = parseInt(this.dataset.taskId);
            const status = parseInt(this.value);

            const previousValue =
                this.dataset.previousValue;

            const messageElement =
                this.parentElement.querySelector(
                    ".task-status-message");

            this.disabled = true;

            messageElement.textContent = "Updating...";
            messageElement.className =
                "task-status-message mt-1 d-block text-muted";

            try {
                const response = await fetch(
                    "/api/tasks/status",
                    {
                        method: "PUT",

                        headers: {
                            "Content-Type": "application/json"
                        },

                        body: JSON.stringify({
                            taskId: taskId,
                            status: status
                        })
                    });

                if (!response.ok) {
                    throw new Error(
                        "Status update failed.");
                }

                const result =
                    await response.json();

                const taskCard =
                    document.getElementById(`task-card-${taskId}`);

                if (taskCard) {
                    taskCard.classList.remove(
                        "task-todo",
                        "task-inprogress",
                        "task-completed"
                    );

                    switch (status) {
                        case 1:
                            taskCard.classList.add("task-todo");
                            break;

                        case 2:
                            taskCard.classList.add("task-inprogress");
                            break;

                        case 3:
                            taskCard.classList.add("task-completed");
                            break;
                    }

                    const taskTitle =
                        document.getElementById(`task-title-${taskId}`);

                    if (taskTitle) {
                        if (status === 3) {
                            taskTitle.classList.add("task-title-completed");
                        }
                        else {
                            taskTitle.classList.remove("task-title-completed");
                        }
                    }
                }

                this.dataset.previousValue =
                    this.value;

                messageElement.textContent =
                    "Status updated.";

                messageElement.className =
                    "task-status-message mt-1 d-block text-success";

                console.log(
                    "Task updated:",
                    result);
            }
            catch (error) {

                this.value = previousValue;

                messageElement.textContent =
                    "Status update failed.";

                messageElement.className =
                    "task-status-message mt-1 d-block text-danger";

                console.error(error);
            }
            finally {
                this.disabled = false;

                setTimeout(() => {
                    messageElement.textContent = "";
                }, 2500);
            }
        });
    });
});