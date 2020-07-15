# Stylometry-Analysis
This software is an implementation of Author's stylometric change detection algorithm for English language. It can be used in text forensics. 

About:
This software implements the sub-task of PAN 2017 Author
Identification, which is to detect style breaches for unknown number of authors
within a single document in English. The presented model is an unsupervised
approach that will detect style breaches and mark text boundaries on the basis
of different stylistic features. This model will use some classical stylistic
features like POS analysis and sentence lexical analysis. Also some new
features naming common English word frequencies within sentence text,
sentence expression and sentence attitude have been proposed. The new
features may not be directly linked to author’s style of writing but to the
subject/topic of sentence under analysis. Moreover the model uses sentence
window for style detection. The sentence window may be extended to
neighboring sentences during its unsupervised analysis. 

Implementation:
The software uses open NLP library for extracting specific N-Grams for Part of Speech analysis.
This software is implemented in C# .net
Currently 60 text documents are provided for upload size reson in startuppath/pan directory. The software can 
process each document individually and compute the style change on text window basis. The text boundaries where 
style change detected are put into startuppath/Results directory.
Different stylistic n-grams expressing different expressions of writer are included in startuppath/txtDir in .dat format.
The 5000 most used word n-grams with different frequencies have also been included.

Users can specify their input directories with -i option and the output directories can be specified with -o option.

The complete details of model are available at: http://ceur-ws.org/Vol-1866/paper_106.pdf

Anyone needs to research over text forensic can use the software.
