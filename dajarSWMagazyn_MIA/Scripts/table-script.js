document.addEventListener("DOMContentLoaded", function() {
    const tables = document.querySelectorAll('.table-mobile table');
    
    tables.forEach(function(table) {
        const headers = table.querySelectorAll('tbody th');
        const rows = table.querySelectorAll('tbody tr');
        
        rows.forEach(function(row) {
            const cells = row.querySelectorAll('td');
            
            cells.forEach(function(cell, index) {
                const headerText = headers[index].textContent.trim();
                cell.setAttribute('data-label', headerText);
            })
        });
    });
});

const toggleMenu = () => {
    const menu = document.querySelector(".navigation ul");
    menu.classList.toggle('active');
}

