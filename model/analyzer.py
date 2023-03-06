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


def main():
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

    


if __name__ == '__main__':
    main()
