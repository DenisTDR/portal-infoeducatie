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
    var height = $(".judging-table-wrapper").height() - 120 + "px";
    // console.log(height);
    $(".judging-table-wrapper .dataTables_scrollBody").css({height: height, 'max-height': height})
    judgingTable.api().draw();

    initializeJudgingInputFields();
    initializeVerticalTabNavigation();
    initializeForcedFocusOnInput();
}

function initializeVerticalTabNavigation() {
    $('.judging-table .input-cell input').keydown(function (e) {
        if (e.keyCode !== 9) {
            return;
        }
        e.preventDefault();
        var _this = $(this);
        var tdIndex = _this.closest("td").index();
        var crtTr = _this.closest("tr");
        var targetTr = e.shiftKey ? crtTr.prevAll("tr.inputs-row:first") : crtTr.nextAll("tr.inputs-row:first");
        targetTr.children().eq(tdIndex).find("input").focus()
    });
}

function initializeForcedFocusOnInput() {
    $('.judging-table .input-cell input').click(function () {
        var _this = $(this);
        if (!_this.is(':focus')) {
            _this.focus();
        }
    });
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
            _this.val(value);
            setPointsFor(cell, cell.data('criterionId'), cell.data('projectId'), value, function () {
                _this.data('value', value);
            });
        }
    });
    $('.judging-table').on('click', ".input-cell.saving-error", function (e) {
        $(this).closest('td').find('input').blur();
    });
}

function setPointsFor(cell, criterionId, projectId, points, successCallback) {
    var data = {criterionId: criterionId, projectId: projectId, points: points};
    hideErrorIcon(cell);
    addSpinnerElem(cell);
    $.ajax({
        type: "POST",
        url: "/Judging/SetPoints",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            hideLoadingSpinner(cell);
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
            successCallback();
        },
        error: function (errMsg) {
            hideLoadingSpinner(cell);
            addErrorIcon(cell);
            console.log(errMsg);
            var criterionName = $("[data-e-type=criterion][data-e-id=" + criterionId + "]").html();
            var projectName = $("[data-e-type=project][data-e-id=" + projectId + "]").html();
            var msg = "A apărut o eroare la salvarea punctajului pentru proiectul '" + projectName + "', criteriul '" + criterionName + "'. Vezi câmpul roșu și încearcă din nou.";
            alert(msg);
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

function addSpinnerElem(cell) {
    var spinnerEl = $("<div class='saving-icon'><i class='fa fa-cog fa-spin fa-2x fa-fw'></i></div>");
    cell.append(spinnerEl);
    spinnerEl.hide();
    spinnerEl.fadeIn();
}

function hideLoadingSpinner(cell) {
    var spinnerEl = cell.find('.saving-icon');
    spinnerEl.fadeOut(function () {
        spinnerEl.detach();
    });
}

function addErrorIcon(cell) {
    cell.addClass("saving-error");
    var errorEl = $("<div class='error-icon'><i class='fa fa-bug fa-2x fa-fw'></i></div>");
    cell.append(errorEl);
    errorEl.hide();
    errorEl.fadeIn();
}

function hideErrorIcon(cell) {
    cell.removeClass("saving-error");
    var errEl = cell.find('.error-icon');
    if (!errEl.length) return;
    errEl.fadeOut(function () {
        errEl.detach();
    });
}