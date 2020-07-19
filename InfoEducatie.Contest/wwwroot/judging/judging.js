var judgingTable = $(".judging-table");
initializeTable();

function initializeTable() {
    judgingTable = judgingTable.dataTable({
        paging: false,
        scrollX: true,
        scrollY: '400px',
        fixedHeader: {
            header: true,
        },
        fixedColumns: {leftColumns: 2},
        columnDefs: [
            // { width: 'auto', targets: 0 },
            {width: 20, targets: 1},
            {width: 150, targets: '_all'},
        ],
        // autoWidth: false,
        sort: false,
        colReorder: true,
        filter: false,
        bInfo: false
    });
    $('body').popover({
        selector: '.judging-table-wrapper [data-toggle="popover"]',
        html: true,
        trigger: 'hover',
        boundary: 'window',
    })
    var height = $(".judging-table-wrapper").height() - 160 + "px";
    $(".judging-table-wrapper .dataTables_scrollBody").css({height: height, 'max-height': height})
    judgingTable.fnDraw();
}

$('.judging-table .input-cell input').blur(function () {
    var cell = $(this).closest('td');
    var _this = $(this);
    var oldValue = _this.data('value');

    var value = parseInt(_this.val());
    var max = parseInt(this.max);
    var min = parseInt(this.min);
    window.ceva = _this;
    if (value && value !== oldValue) {
        window.ceva = this;
        if (value > max) {
            alert('The value should be between ' + min + ' and ' + max + '. Will set ' + max + '.');
            _this.val(value = max);
        }
        if (value < min) {
            alert('The value should be between ' + min + ' and ' + max + '. Will set ' + min + '.');
            _this.val(value = min);
        }
        _this.data('value', value)
        _this.val(value);
        setPoints(cell, cell.data('criterionId'), cell.data('projectId'), value);
    }
});

function setPoints(cell, criterionId, projectId, points) {
    // console.log(projectId + ' for ' + criterionId + '  update to ' + points);
    var data = {criterionId: criterionId, projectId: projectId, points: points}
    var spinnerEl = $("<div class='saving-icon'><i class='fa fa-cog fa-spin fa-2x fa-fw '></i></div>");
    cell.append(spinnerEl);
    spinnerEl.hide();
    spinnerEl.fadeIn();
    $.ajax({
        type: "POST",
        url: "/Judging/SetPoints",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            hideLoadingSpinner(spinnerEl);
            // console.log(data);
            $(".project-total-points[data-project-id=" + projectId + "]").html(data);
        },
        failure: function (errMsg) {
            hideLoadingSpinner(spinnerEl);
            alert('error');
            alert(errMsg);
        }
    });
}

function hideLoadingSpinner(spinnerEl) {
    spinnerEl.fadeOut(function () {
        spinnerEl.detach();
    });
}