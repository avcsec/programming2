from flask import Flask
from flask import abort
from flask import redirect
from flask import render_template
from flask import request
import json
import requests

app = Flask(__name__)

@app.route('/')
def index():
    r = requests.get('http://127.0.0.1:5000/api/history', verify=False)
    res = json.loads(r.text)
    return render_template("index.html", res=res)

@app.route('/go', methods=['POST'])
def go():
    if request.form.get('type') == 'chart':
        formula = request.form.get('formula')
        from_x = request.form.get('from')
        to_x = request.form.get('to')
        n = request.form.get('n')
        return chart(formula, from_x, to_x, n)
    elif request.form.get('type') == 'calculate':
        formula = request.form.get('formula')
        x = request.form.get('x')
        return result(formula, x)
    else:
        abort(401)

def chart(formula, from_x, to_x, n):
    formula_enc = formula.replace("+", "%2B")
    r = requests.get("http://localhost:5000/api/calculate/xy?formula=" + formula_enc + "&from=" + from_x + "&to=" + to_x + "&n=" + n, verify=False)
    try:
        res = json.loads(r.text)
        x = []
        y = []
        for q in res['results']:
            x.append(q['x'])
            y.append(q['y'])
            if q['y'] == "NaN" or q['y'] == "Infinity":
                return render_template("error.html", formula=formula)
        return render_template("chart.html", formula=formula, x=x, y=y)
    except:
        return render_template("invalid_formula.html")

def result(formula, x):
    formula_enc = formula.replace("+", "%2B")
    r = requests.get('http://127.0.0.1:5000/api/calculate?x=' + x + "&formula=" + formula_enc, verify=False)
    try:
        res = json.loads(r.text)
        r = res["result"]
        if r == "NaN" or r == "Infinity":
            return render_template("error.html", formula=formula)
        return render_template("result.html", formula=formula, x=x, r=r)
    except:
        return render_template("invalid_formula.html")

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=80)