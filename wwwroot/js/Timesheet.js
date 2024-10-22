document.addEventListener('DOMContentLoaded', function () {
    const totalDays = 7;

    for (let row = 0; row < totalDays; row++) {
        setupEventListeners(row);
    }

    function setupEventListeners(row) {
        const selectAuditActivity = document.getElementById(`SelectAuditActivity${row}`);
        const selectDynamicOptions = document.getElementById(`SelectDynamicOptions${row}`);

        if (selectAuditActivity) {
            selectAuditActivity.addEventListener('change', function () {
                const selectedOption = selectAuditActivity.value;
                console.log(`Row ${row}: Selected option is ${selectedOption}`);
                loadOptions(selectedOption, row);
            });
        } else {
            console.error(`Dropdown SelectAuditActivity${row} not found.`);
        }
    }

    function loadOptions(selection, index) {
        if (selection) {
            const handler = selection === 'Audit' ? 'LoadAudits' : 'LoadActivities';
            const dateElement = document.getElementsByName(`Timesheets[${index}].Date`)[0];
            const date = dateElement ? dateElement.value : '';
            console.log(`Loading options for ${selection} on date ${date}`);

            fetch(`/Timesheet?handler=${handler}&date=${date}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`HTTP error! Status: ${response.status}`);
                    }
                    return response.json();
                })
                .then(data => {
                    console.log(`Data received for ${selection}:`, data);
                    populateDynamicOptions(data, selection, index);
                })
                .catch(error => {
                    alert(`Error fetching ${selection.toLowerCase()} data: ${error.message}`);
                    console.error(`Error fetching ${selection.toLowerCase()} data:`, error);
                });
        } else {
            clearDynamicOptions(index);
        }
    }

    function populateDynamicOptions(data, selection, index) {
        const selectDynamicOptions = document.getElementById(`SelectDynamicOptions${index}`);
        selectDynamicOptions.innerHTML = '<option value="">- Select -</option>'; // Default empty option

        if (Array.isArray(data)) {
            data.forEach(item => {
                const option = document.createElement('option');
                option.value = item.id;
                option.textContent = item.activity || item.auditTitle; // Adjust according to your data structure
                selectDynamicOptions.appendChild(option);
            });

            selectDynamicOptions.name = selection === 'Audit'
                ? `Timesheets[${index}].AuditID`
                : `Timesheets[${index}].Activity`; // Assign correct name based on type
        } else {
            console.error('Unexpected data format:', data);
        }
    }

    function clearDynamicOptions(index) {
        const selectDynamicOptions = document.getElementById(`SelectDynamicOptions${index}`);
        selectDynamicOptions.innerHTML = '<option value="">- Select -</option>'; // Default empty option
        selectDynamicOptions.name = ''; // Clear the name attribute
    }
});
