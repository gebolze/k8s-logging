#!/bin/bash

echo "------------------------------------------------------------------------"
echo "Setup the local demonstration cluster"
echo "------------------------------------------------------------------------"

kind create cluster --name k8s-logs \
  --config kind-cluster-config.yaml

kubectl cluster-info --context kind-k8s-logs

echo "------------------------------------------------------------------------"
echo "Setup nginx ingress controller"
echo "------------------------------------------------------------------------"

#kubectl apply -f \
#  https://raw.githubusercontent.com/kubernetes/ingress-nginx/master/deploy/static/provider/kind/deploy.yaml


#kubectl wait --namespace ingress-nginx \
#  --for=condition=ready pod \
#  --selector=app.kubernetes.io/component=controller \
#  --timeout=300s

echo "------------------------------------------------------------------------"
echo "Setup elk stack"
echo "------------------------------------------------------------------------"

kubectl apply -f ./elk-logging/namespace.yaml

kubectl apply -f ./elk-logging/elasticsearch.yaml
kubectl rollout status sts/es-cluster --namespace=elk-logging

kubectl apply -f ./elk-logging/kibana.yaml
kubectl rollout status deployment/kibana --namespace=elk-logging

kubectl apply -f ./elk-logging/logstash.yaml
kubectl rollout status deployment/logstash --namespace=elk-logging

kubectl apply -f ./elk-logging/ingress.yaml

echo "------------------------------------------------------------------------"
echo "setup logs collection using fluent-bit"
echo "------------------------------------------------------------------------"

kubectl apply -f ./kube-logging/namespace.yaml
kubectl apply -f ./kube-logging/fluentbit.yaml
kubectl rollout status daemonset/fluent-bit -n kube-logging

echo "------------------------------------------------------------------------"
echo "build and publish demo workload with esc common schema logging"
echo "------------------------------------------------------------------------"

cd demo-web
docker build . -t gebolze/demo-webapp
docker push gebolze/demo-webapp
cd ..

echo "------------------------------------------------------------------------"
echo "deploy demo workload"
echo "------------------------------------------------------------------------"

kubectl apply -f demo-webapp-ecs.yaml
kubectl apply -f demo-webapp-json.yaml
kubectl apply -f demo-webapp-text.yaml

kubectl rollout status deployment/demo-webapp -n ecs
kubectl rollout status deployment/demo-webapp -n json
kubectl rollout status deployment/demo-webapp -n text
