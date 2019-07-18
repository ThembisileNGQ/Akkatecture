#!/usr/bin/env bash

while getopts testrunner:tests:log-folder:coverage: option
do
    case "${option}" in
        testrunner) TESTRUNNER=${OPTARG};;
        test) TESTS=${OPTARG};;
        log-folder) LOGFOLDER=${OPTARG};;
        coverage) COVERAGE=$OPTARG;;
    esac

echo  "$TESTRUNNER"
done
