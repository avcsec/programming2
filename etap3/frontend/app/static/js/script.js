function radioChanged()
{
    var radio = document.getElementsByName('type');
    for (var i = 0; i < radio.length; i++) {
        if (radio[i].checked) {
            if(radio[i].value == "chart")
            {
                setChart();
            }
            else if(radio[i].value == "calculate")
            {
                setCalculate();
            }
            else
            { }
            break;
        }
    }
}

function setChart()
{
    document.getElementById('x_div').style.display = "none";
    document.getElementById('x').removeAttribute("required");
    document.getElementById('from_div').style.display = "block";
    document.getElementById('from').setAttribute("required", "");
    document.getElementById('to_div').style.display = "block";
    document.getElementById('to').setAttribute("required", "");
    document.getElementById('n_div').style.display = "block";
    document.getElementById('n').setAttribute("required", "");
    document.getElementById('go_button').innerText = "Generuj wykres";
    document.getElementById('chart_radio').checked = true;
    document.getElementById('calculate_radio').checked = false;
}

function setCalculate()
{
    document.getElementById('x_div').style.display = "block";
    document.getElementById('x').setAttribute("required", "");
    document.getElementById('from_div').style.display = "none";
    document.getElementById('from').removeAttribute("required");
    document.getElementById('to_div').style.display = "none";
    document.getElementById('to').removeAttribute("required");
    document.getElementById('n_div').style.display = "none";
    document.getElementById('n').removeAttribute("required");
    document.getElementById('go_button').innerText = "Oblicz";
    document.getElementById('chart_radio').checked = false;
    document.getElementById('calculate_radio').checked = true;
}

function setParams(formula, x, from, to, n)
{
    if(formula !== 'None')
        document.getElementById("formula").value = formula;
    if(x !== 'None')
    {
        document.getElementById("x").value = x;
        setCalculate();
    }
    else
    {
        setChart();
    }
    if(from !== 'None')
        document.getElementById("from").value = from;
    if(to !== 'None')
        document.getElementById("to").value = to;
    if(n !== 'None')
        document.getElementById("n").value = n;
}