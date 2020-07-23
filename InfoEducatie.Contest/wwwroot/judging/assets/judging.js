var judgingTable = $(".judging-table");


function initializeJudgingTable() {
    judgingTable = judgingTable.dataTable({
        paging: false,
        scrollX: true,
        scrollY: '400px',
        fixedHeader: {
            header: true,
        },
        fixedColumns: {leftColumns: 2},
        columnDefs: [
            {width: 250, targets: 0},
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
        selector: '.judging-table-wrapper [data-toggle="popover"][data-content][data-content!=""]',
        html: true,
        trigger: 'hover',
        boundary: 'window',
        template: '<div class="popover judging-cell-popover" role="tooltip"><div class="arrow"></div><h3 class="popover-header"></h3><div class="popover-body"></div></div>'
    })
    var height = $(".judging-table-wrapper").height() - 160 + "px";
    $(".judging-table-wrapper .dataTables_scrollBody").css({height: height, 'max-height': height})
    judgingTable.fnDraw();

    initializeJudgingInputFields();
}

function initializeJudgingInputFields() {
    $('.judging-table .input-cell input').blur(function () {
        var cell = $(this).closest('td');
        var _this = $(this);
        var oldValue = _this.data('value');

        var value = parseInt(_this.val());

        if (!_this.val() || isNaN(value)) {
            return;
        }

        var max = parseInt(this.max);
        var min = parseInt(this.min);
        window.ceva = _this;
        if (value !== oldValue) {
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
            setPointsFor(cell, cell.data('criterionId'), cell.data('projectId'), value);
        }
    });
}

function setPointsFor(cell, criterionId, projectId, points) {
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
            updateContentWithEffect($(".project-total-points[data-project-id=" + projectId + "]"), data.total);
            // $(".project-total-points[data-project-id=" + projectId + "]").html(data.total);
            // console.log(data.sections);
            if (data.sections && data.sections.length) {
                for (var i = 0; i < data.sections.length; i++) {
                    var section = data.sections[i];
                    // console.log(section)
                    updateContentWithEffect($(".project-section-points[data-project-id=" + projectId + "][data-section-id=" + section.id + "]>div"), section.points);
                    // $(".project-section-points[data-project-id=" + projectId + "][data-section-id=" + section.id + "]").html(section.points);
                }
            }
        },
        failure: function (errMsg) {
            hideLoadingSpinner(spinnerEl);
            alert('error');
            alert(errMsg);
        }
    });
}

function updateContentWithEffect(element, newValue) {
    var oldValue = element.html().trim();
    if (oldValue === newValue.toString()) {
        return;
    }
    window.tdr = element;
    element.slideUp(500, function () {
        element.html(newValue);
        element.slideDown(500);
    });
}

function hideLoadingSpinner(spinnerEl) {
    spinnerEl.fadeOut(function () {
        spinnerEl.detach();
    });
}