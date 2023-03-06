# supress any conda or numpy warnings
import warnings
warnings.filterwarnings("ignore")


import sys
import joblib
import pandas as pd
from sklearn.impute import SimpleImputer
from sklearn.compose import ColumnTransformer
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import StandardScaler
from sklearn.preprocessing import OneHotEncoder
from sklearn.linear_model import LogisticRegression

from google.cloud import storage

import tempfile

def hello_world(request):
    """Responds to any HTTP request.
    Args:
        request (flask.Request): HTTP request object.
    Returns:
        The response text or any set of values that can be turned into a
        Response object using
        `make_response <http://flask.pocoo.org/docs/1.0/api/#flask.Flask.make_response>`.
    """
    try:
        storage_client = storage.Client()
        bucket = storage_client.get_bucket("gcf-sources-51489756661-us-central1")
        pp = tempfile.gettempdir() + "/please.pkl"
        blob = bucket.blob("fd_prod_tr.pkl")
        blob.download_to_filename(pp)
        m_cls_import = joblib.load(pp)
        #return str(type(m_cls_import))
    except Exception as e:
        print(e)
        return str(e)

    request_json = request.get_json()
    #if request.args and 'message' in request.args:
    #    return request.args.get('message')
    #elif request_json and 'message' in request_json:
    #    return request_json['message']
    #else:
    #    return f'Hello World!'
    print(request_json)
    column_list = ['V3', 'V4', 'V8', 'V9', 'V10', 'V11', 'V18', 'V12', 'V13' ,'V15', 'V17', 'V19', 'V2', 'V5', 'V6']
    casting_array = [int, int, int, int, int, float, str, float, str, float, float, str, int, int, int]
    data_dict = {}
    for i, cl in enumerate(column_list):
        data_dict[cl] = [casting_array[i](request_json[cl])]

    dataframe = pd.DataFrame.from_dict(data_dict)
    prediction_result = m_cls_import.predict_proba(dataframe)
    return str(prediction_result[0])
    


def foomain():
    cls_import = joblib.load('fd_prod_tr.pkl')
    column_list = ['V3', 'V4', 'V8', 'V9', 'V10', 'V11', 'V18', 'V12', 'V13' ,'V15', 'V17', 'V19', 'V2', 'V5', 'V6']
    casting_array = [int, int, int, int, int, float, str, float, str, float, float, str, int, int, int]
    data_dict = {}

    if (len(sys.argv) - 1 != len(column_list)):
        print('-1')
    
    arguments = []
    for i in range(len(sys.argv)):
        if (i >= 1):
            arguments.append(casting_array[i - 1](sys.argv[i]))

    
    for i in range(len(column_list)):
        data_dict[column_list[i]] = [arguments[i]]
    
    dataframe = pd.DataFrame.from_dict(data_dict)
    prediction_result = cls_import.predict_proba(dataframe)
    print(prediction_result[0])
    
