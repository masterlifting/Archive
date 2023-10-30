```mermaid
graph TB
    CID[Client]
    CID -->|kubectl apply| MS(Master)
    MS -->|schedule| NS1(Worker Node 1)
    MS -->|schedule| NS2(Worker Node 2)
    NS1 -->|run| P11[Pod 1.1]
    P11 --> C11[Container 1.1]
    P11 --> D11[Dapr Sidecar 1.1]
    NS1 -->|run| P12[Pod 1.2]
    P12 --> C12[Container 1.2]
    P12 --> D12[Dapr Sidecar 1.2]
    NS2 -->|run| P21[Pod 2.1]
    P21 --> C21[Container 2.1]
    P21 --> D21[Dapr Sidecar 2.1]
    NS2 -->|run| P22[Pod 2.2]
    P22 --> C22[Container 2.2]
    P22 --> D22[Dapr Sidecar 2.2]
    
```

We use Keycloak as a central identity provider for the system.
The JWT bearer token is forwarded with every call.


```mermaid
graph TD
    client[Client]
    node1[(Worker Node 1)]
    node2[(Worker Node 2)]
    node3[(Worker Node 3)]
    master[Master Node]
    apisix[APISIX]
    keycloak[Keycloak]
    
    client -- Request --> apisix
    apisix -- Authenticate User --> keycloak
    keycloak -- Returns JWT Bearer --> apisix
    apisix -. JWT Bearer .-> master

    master -- Schedule Pods in Nodes --> node1
    master -- Schedule Pods in Nodes --> node2
    master -- Schedule Pods in Nodes --> node3

    apisix -. JWT Bearer .-> node1 
    apisix -. JWT Bearer .-> node2
    apisix -. JWT Bearer .-> node3 

    node1 -- Responds with Pod Status --> master
    node2 -- Responds with Pod Status --> master
    node3 -- Responds with Pod Status --> master

    master -- Communicates Pod Status --> apisix
    apisix -- Responds with Appropriate Response --> client
```

If you goto the [Who Am I](https://whoami.feuerfest.dev/) service on the development cluster, you can see what headers are forwarded to the service.

In the figure below, an example is shown.

```console
Hostname: whoami-7685bb7cf4-xw9sk
IP: 127.0.0.1
IP: ::1
IP: 10.42.4.21
IP: fe80::7003:b4ff:fe8d:1db2
RemoteAddr: 10.42.4.2:41572
GET / HTTP/1.1
Host: whoami.feuerfest.dev
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
Accept-Encoding: gzip, deflate, br
Accept-Language: de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7,nl-NL;q=0.6,nl;q=0.5
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJRX0ROMHA3Z0hzdjN0UzBLTWtPMVdPc3lfeUF1VEZ2ODE3M1BEZENKZVgwIn0.eyJleHAiOjE2OTEzOTQxOTksImlhdCI6MTY5MTM5Mzg5OSwiYXV0aF90aW1lIjoxNjkxMzkzMDIxLCJqdGkiOiI1MmI4NzQ0ZS1mZTY1LTQ0MjYtYjAyNC02MGE1NTY2YmE2NjIiLCJpc3MiOiJodHRwczovL2F1dGguZmV1ZXJmZXN0LmRldi9yZWFsbXMvZmV1ZXJmZXN0IiwiYXVkIjoiYWNjb3VudCIsInN1YiI6IjQxMzAwYTM2LWJhYzMtNGYyOC05MTdkLWQyZTFkNWU5MDE0NCIsInR5cCI6IkJlYXJlciIsImF6cCI6ImRhc2hib2FyZHMiLCJub25jZSI6IjlmYjZiZTY5YTFhMTYxYWE0Nzg4MzMzMDM4MGZlOWJhIiwic2Vzc2lvbl9zdGF0ZSI6Ijg5ZWI4MTM2LTZlOTYtNDU0MC04Y2E1LThkNjI0YTg3YmI0YyIsImFjciI6IjAiLCJyZWFsbV9hY2Nlc3MiOnsicm9sZXMiOlsiZGVmYXVsdC1yb2xlcy1mZXVlcmZlc3QiLCJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCIsInNpZCI6Ijg5ZWI4MTM2LTZlOTYtNDU0MC04Y2E1LThkNjI0YTg3YmI0YyIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwicHJlZmVycmVkX3VzZXJuYW1lIjoiZmV1ZXJmZXN0YWRtaW4iLCJlbWFpbCI6ImFkbWluQGZldWVyZmVzdC5kZXYifQ.pgWHZPp6YAf21do1Pp6tS-Pd9NmG7zdr5mg0feLSJBh4d8mQPOGt3kaJHxw1HWMYt2WPa8UIUs0o4SU23yJn6KhDXzv8W72ivE2yHN4ja8_12TfWw4EmXkcFThdboEIBZjSMdbbD37yuk8s0Bmxvbas2zY7AcdY-h03soEP4mVLOODfoT3Mo_ajZiWIW11T0ZQthJ6R7SpTjAUyiya1RgiVJcCoZjVWoU5YZBQ_TnvzMSAlVwixlXO4nwcCYW5cZLYDCrFv2cC5ZkD5H6aLhATilJT4hnYz7tW3k-AN7okGHqTrflKfIwAk33Vz2uCtdukVwZay61KxekbMFWIR7AQ
Cache-Control: max-age=0
Cookie: session=HRo8OLiF5xknl3KhcC1YUA|1691397499|mM4TXXauJtzTp-L3ot4gOEV52TI4XO8OfWc-R4U-1A4TIt5IDu2hx2UX5oFH_a4CeHZH203fGQUzMdLv_E9MxmInFDBUN8mshAzE1rgTYbs2aiioIs8c3NULmeWTOeVNZEUB2-sTDI6uThh2YlTgmlLyf5SpTF1U5NZRbiZTginaFo5iu8go8AGQs_cQnBIWZeQE3vyyo3E0qQRk-xYwuO1EYbCS65ezj8j4WJrpu5uMQIDAP9I8JFc0zWhOGZa29E6s1m_Jf6O1lBUywb-0mSVQwUqKFIsXUUH73tlsV9Nr8PVK_zEa1s6zV4h4PzinaX1tZ2KTq4T1L5OEE8t14yzjp8Tjpmc0pcWF56B6NVgHmunDAuX1E1n33JS2GEEomh_E0AcxT02kdDVB3H8UZmvP-DJyOYDtqflj1SJrpeUo6oLlXbJZoEsc3bnQfLv9pT2NWeJbfxXm4mv5YFCSLX78gQU15jUwuPK9EXsfRLbIPG1E39tPECL0ywGjFdhq8AT790uxS9LESS1UYHiD_LWaJAXT9XEYnqgx6LdDT0N_ZRrFf0kFQ8bT3rzRKjIzxGSeuIdCNSG83u2sVguq0GejcYulq9uSeKwXFiWRKuYuLALna5l08G-gQCC4IsUbZA1rb2oHeteUxSv5zH7jMfsxZ7Kc-F_0YTpYKsd6e_FvvXrIx8-MJU4o3mRsOrmN3qscwHh1OL11VNhsjdGjrxNFCzEjljCJ2uAt_rCDHq1Lg_1I56GHqE8PK8QjAJAn2qGy3kQQgBKI3UNGSbYQHmC7gUlxkz_hBO0ayCjF9KVRzzIQDNqePHK8kFv23A8Ze_Qyk3BogKPm5H6acGYNMhGuF7pI_y2GqgIJZpzvi757_M5Jb3LIcAVTuPNwj66a6vx6jKFMuWm16_ZfcNNdByStNHb3qzBV61Haj57BOBAAiXNlqbRGGx8hPSVlW2KdXwCcSHlPyx47xHWY8UhiO_P-5ycnHafUPDwcwwDfsMOFPc3db-S6OdiOWxolLcagKT7bt8GUy1xnCCnCKMv0zJpMe8kQEYf2AMZQf8OLhK5ZsMewJd5ysdhc4Xcc8gVdHbvYV7Z68uaq7HQp_VtbdbSTd7FWM0KA3i-TsvrSC6eQ5flzaXhvfqZ11yMtNksPA0vbe1KCjtwjblgTAkT7Aq3DZflzdiwe3fTouO-v_uDCVaz-4t_GyrrwWgUStzgTRExjNIJrr7VkC6AIGZIi03NMPqUCXQsZdODu0x4XXs1FCNLl7I17nd_CDDIg0pcc0sVnL3e74vaWM4A5fAJpEnfQuEZKRulMqWYOmSxwv1dzfZmLYPQT30DT8YmVma9h-zpPUDpVEP7ej-zLNE8HUenYd70yYwj5AD90Dm8-H_AGRKxULVwdLJOSJrxo81XAbREYu_Ma7TS_8IWye7wtzL_n91EwpCWLhYsbHVu63OzZ2sOInfsKgFVZ6bX42zxqJ1tfoYVe-QWWrGEGN1S_6IQmHEt7fwmMhsNPaQOUYncZGXp1Ko9JJgnulPO28zLLyIDkZxOyi8NBZpxLOtprGB_CXiOl2I4VeC_B5rkOaouoM5geFkg2j6V_Yf4VCAPiVrKY-v15rnkmu_N9qJujg-JhjxQ3UghgLTxBrFmawBtf7Z-B14pmsTekQHJIhHmdhI0D1iflYq5zTwujmqFaIzIKEIcy-erECPmGChpyfwJlj3DUHQoymGgswIjzmMg1ahBwkjThk6Ed4Xisw87_8O0fiKqSmamlqv46gR-gNWGj_KyJU1AiFrnrg_eUj12UqTHCf-6fW4aG8X_i8fTt1PIYGzFNXFh67pzmuER5InqkqYBotjU6ylXPybiyUJ6VvMNm56R3nhhIt8OG3T5a7720PTWAnS-rfxbUX1SyiWszgTLTXLjb65bUF2F0t4uutDqG6B64GxLgAwJdj3prnydcbYzjg68xfbe9c79UNvqGYw48VKEhBO9ttRf6toBeDNEYujZXHSrCsRzaFK1qWphzXPICV6sDC8STr5ZSInLWG6sRKntZ5w-_rgNx8chQ4KvwYUvVGNnMjBzVsKqxZh9m_Pr9oYJsv45K72O0Io8sijEuqUsMlxxskd-4ZF2wV_iP5_BYRAjS74HOJfD-NDphFDFQBKx7lEpWF2L735g6MqTBI_U8O-xAgJuDkeNTVASkZkAv36uwDTskEFxSytBCj0ILW_UJ6lyXvdM1VAavDgU3G3Q4EZB3hiAy3LrM1grShn-fUT69WULTbnt3Eu-3E1U1HWH_oDsOsow_m8VBkKEcQ3tCP700_33NQjlEtrdhOCii2RZ_YmBs-KFhwf0Z8f9qz3YXL2_ivz8j-eYPeqlPr5XjBONKACotWyyBBUVqi5NO4ggs4gsKL6IgKeC2YnI-0nEdyj2WPwQIujYcFJmsyLtwa-LVzGMIYlz54Ez-sogp2yu1rMCkEaAj9lioBusywxhGy1qf2TvJeFrgLDx9FKyAtQM7fAwHiQwOmHYD2pHNloByMzffuJY3_yDmwf8DAoEgldbMfpufHQGQgnG01HwKVGBVKzKocum_llMoNirh1tl8bQXH4My0QD8eRBJBwcIyzpzZM6fyWfMjcts_djqUbvPSAozqpTX25SEZzqc-eIWTWBWF6HaLGoQEiCnjgoF2A_akk9maJP2fzTh4r_tdp6X33Ld4B_GDdklECpMwMCAkzNjsTLXnCk3hhw0SmFxTtZwFbzR96UJQPoD7L-Q841d-DMUlPxcluP-fDthKc5Xm538cDKZAhRXO8YlL8VIRast0EnF4BEGosGZ3NTNXMqU451fAXvXG9DhUEPrIqaY_JmV-TVYQcUr-5oi4T78yUEejQZ88FQfss5_Tpuf0BnzwouojwMmKYmmHNPJXlyfS1EZz6OfDwPv4Z1bxnrKYx8mmBhe7bwWzq14oS_fQJfUzGHDtm52SaaYBBHlyYwU03O8rLhtzKKmF-IkfqVjyE8fIixV8rFR9WyFFRVg6O3orsPqWo8xYdmv4eUNkSSedocWaB-5L-IWL_5b3eAutY5Fe-ld5iPSXoSlbtZF2ySRdKtZYFPOauOFJ48SYKbdKSwZI0mVpW2tvw6NXloQHnKSV5hpO-X9Wwzml3ldVqh903tsGv_3krZvl2SRVbOozxzucn8nwlWkirHec6OsJbpiVpqzlmV0ICWnDIcfC_5y82AOjAtA2LiF9sQABFJk_3xOEZJt9Wj9AEuI41lKKZ1h8E-t2E0hgvhDd5pa8_x57_glgJ-bNKa_0t8TvRa2jQG5mTbqQTIjTrmYkwAZPANjJ-qSw2tr8KfwIpT25zlQEdhzSLDSYlC_euijl30vtz1oKcSYMmQnXLeh2HtJt3WIgqTFsUjWOVEbsS9YUniAjr7cFAAHa_4Qkl_f-mxu0a8GTsBUOQwh_Y2RFKeCliSDllRaca4ohOc2kC3iwZqFK1DfOYVvYQ2P9gjU5EQy0b-svq1olxDsZXYCjgCLG5L-dlA6QRMu5NLzsLJK0qXZhwjhUxHvGb3MlvJoZCxTSxDT4uemcqcz9pg4kW-h12NVC-cB1Sax7HW4vBlt53iIkiwEMlqHmfPWyVWC8fz8BZB524rIhzBFZGnDvbM8ohfQEEYnvgpNhnipjZiRvx1CS6HodXB5VUBPBuobrJr5sZP4_EAXV8go_8HNgEtWCUdfzJLuDEgH9BnN44fMzLnqjPClMIq68xw1mb2Muo8z_YviXgiRDTuxDzn2FuaIk2vd4wA0RWYtEZyGwu88JjhiS22k-HS-mudwTR5ac_MqNM5oMmuxo2whWaXSluOLe49ARrAYcNEOLwHKSs-jzxVk_Qvqf7VdIPLIaWvFu7iWToH4sIPApq4QHGlSR6E8FbJVQhA_teZ9jFUW6NEMfKpfUDcfHEidJ5UQ3D3GZfsaCzD87j8W7TCIBFqRGmuBewrmk-UkHRxqNUGGWi7qt_yK1EB5M580; session_2=KUIppAZ4e06eVGu0r7WjbBt6hqipDiW2qTBZCPv9n3dHCT09uJIpO1DW1l4_1ayjFOuXt0hJzVWhcxjTajVyusbU_xseAvVI9PonhWb3I-ubB-nUw8ste93rC8gNKhMzDI88hMlbiSzpvlD1XwVDvZR9-xgfBLfvfGublsPyt793RTbhII-nZNLbWsLVGZ26iSv6MzTtKW9HzydaFIeUxi3bUaefrKTiJRQSYcAQYlthFEM9lKyxKl0PGpI5w09IOu_ifm5sIBOyIBhxXXN9Nc5E-hf7yxQlHtrgMNxrVjiqdmUZ48rTSFpeAumQ3Vvlup7e0y-qmpTIoXPDZYQzCpToOzgRRpVc2xLJ8osnC8koViQqs1sT6nBLhl5WXywGLJi4IJ5XpOMJfVyoB7F4OMkxwWIHj9Wj2z6rCd8lW7BPAoNjntlcCSQ2p88TLotk3WTg6079UkWVY8c4mocJ_AjWWsNz7fyckm62YXrRxm32XlZ8ayCMKdbfdHzI38D1XZYoZgT42INelhujKQcV0B5SiHegv8CiSDOPUWmy7ABxFzjN0qEXmOhoY0XxnmL8GbI2mSfMQJzFfacH5ftsTIsTceWi8k-MHlk9zbmg5L1ozgZV4qNHYkneNayN4xL18YQd7KjnI1js1WUIjOqnvD0L1Pdk9oxyKPiU8PbaeZNKul5N2nmo9_GPMGwjZoeclW89Cs0VvqZ6VBeL0VErJ3AbrNp8ejvU1OGF2GCM6yiNdruk7lV8t5T5A0u6MFQwDxhYgWo20kiu7hr1Vv16esmHpxRXytByTH4lJhm5so94J9gZ8N4r_OK2szX1u73hLWyiCS8eeEWCDskW6prH9De38YmkGyc3W2jnnUO6erSwF7w9GhGD41ZGEKrdwvzWQmEfEU_lVlClBP-DEPQfUmIEE6Vmu-qbor5LE3c490AAfa01Hr4gcLsDp56O108V796mEat2o1SxsfeB_AbsxEFf1e7kPaZrxmE5yaWAbL2k7WLDNCnRfK2RGtez-9kkT3Zd58_pnX90y0Ip_yWUI49fUkECGHmJFWMXe1rGMgevX6wcm9evVUPonfIBlzVoNsoUzEQ1JNUZjNZLQuBmpD6QrQX8Jn9QnwR23xXXr4C3CJGM43VEG_IMij6yhTyjCu6wmUg6NoopuPlu-nIR1xBMpYdra5oD0XHeecvGmbQtKWhZgAWOkHcKOLBGpEwS4jeejZ51AHb0I-eitoqbvHcIIjHhSFaN7qo2xXLXLVi2k-VAmGOLcKi6xGBtOLectJoCr2he3TvImhEqdeXrOnoXcJjwhnb-OEL-UBypg3_l0po3R6oXblavKki5nMJRs5rv4lSS8F3IkO1i9MDCKj_X1QB8HTSYFYqiKQ0R_s_Sk2RMJcb0NprWvVy0ATBQjpkxx2Q_W4yNzN1AGWW9bjwjl_wZAowCJCEjR4KbwmSTL-VQbu3xtx0NPaPjfbl7hiqCHC1TQkwsLqHK_TIlKWG6cF0r_HcCD65cu0Sk_W0_lchJFW773-HbDBqGnlrz4zOxj8X3tkodq0z_RkIidbqmjyfSccpab9VLW7Qlcuu_Y|fIqWJcCrOkafyg7IwZFv0YE80dA
Sec-Ch-Ua: "Not/A)Brand";v="99", "Google Chrome";v="115", "Chromium";v="115"
Sec-Ch-Ua-Mobile: ?0
Sec-Ch-Ua-Platform: "Windows"
Sec-Fetch-Dest: document
Sec-Fetch-Mode: navigate
Sec-Fetch-Site: same-site
Sec-Fetch-User: ?1
Upgrade-Insecure-Requests: 1
X-Forwarded-For: 95.222.24.224, 10.42.1.0
X-Forwarded-Host: whoami.feuerfest.dev
X-Forwarded-Port: 443
X-Forwarded-Proto: https
X-Id-Token: eyJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsImF1ZCI6ImRhc2hib2FyZHMiLCJzZXNzaW9uX3N0YXRlIjoiODllYjgxMzYtNmU5Ni00NTQwLThjYTUtOGQ2MjRhODdiYjRjIiwiYXV0aF90aW1lIjoxNjkxMzkzMDIxLCJleHAiOjE2OTEzOTQxOTksInByZWZlcnJlZF91c2VybmFtZSI6ImZldWVyZmVzdGFkbWluIiwianRpIjoiMjI0YWU1OGYtNThiOS00OGVjLWI2Y2MtZDRmYTc1OWI5NDg3IiwiaXNzIjoiaHR0cHM6Ly9hdXRoLmZldWVyZmVzdC5kZXYvcmVhbG1zL2ZldWVyZmVzdCIsInN1YiI6IjQxMzAwYTM2LWJhYzMtNGYyOC05MTdkLWQyZTFkNWU5MDE0NCIsIm5vbmNlIjoiOWZiNmJlNjlhMWExNjFhYTQ3ODgzMzMwMzgwZmU5YmEiLCJhY3IiOiIwIiwic2lkIjoiODllYjgxMzYtNmU5Ni00NTQwLThjYTUtOGQ2MjRhODdiYjRjIiwidHlwIjoiSUQiLCJhenAiOiJkYXNoYm9hcmRzIiwiYXRfaGFzaCI6Imd2T2l0eFAzZUlMVWxuRFNzMUhIYXciLCJpYXQiOjE2OTEzOTM4OTksImVtYWlsIjoiYWRtaW5AZmV1ZXJmZXN0LmRldiJ9
X-Real-Ip: 10.42.1.0
X-Userinfo: eyJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsInN1YiI6IjQxMzAwYTM2LWJhYzMtNGYyOC05MTdkLWQyZTFkNWU5MDE0NCIsInByZWZlcnJlZF91c2VybmFtZSI6ImZldWVyZmVzdGFkbWluIiwiZW1haWwiOiJhZG1pbkBmZXVlcmZlc3QuZGV2In0=
```

The Authorization: Bearer content

```json
{
  "alg": "RS256",
  "typ": "JWT",
  "kid": "Q_DN0p7gHsv3tS0KMkO1WOsy_yAuTFv8173PDdCJeX0"
}
```

```json
{
  "exp": 1691394199,
  "iat": 1691393899,
  "auth_time": 1691393021,
  "jti": "52b8744e-fe65-4426-b024-60a5566ba662",
  "iss": "https://${keykloak}/realms/feuerfest",
  "aud": "account",
  "sub": "41300a36-bac3-4f28-917d-d2e1d5e90144",
  "typ": "Bearer",
  "azp": "dashboards",
  "nonce": "9fb6be69a1a161aa47883330380fe9ba",
  "session_state": "89eb8136-6e96-4540-8ca5-8d624a87bb4c",
  "acr": "0",
  "realm_access": {
    "roles": [
      "default-roles-feuerfest",
      "offline_access",
      "uma_authorization"
    ]
  },
  "resource_access": {
    "account": {
      "roles": [
        "manage-account",
        "manage-account-links",
        "view-profile"
      ]
    }
  },
  "scope": "openid profile email",
  "sid": "89eb8136-6e96-4540-8ca5-8d624a87bb4c",
  "email_verified": false,
  "preferred_username": "feuerfestadmin",
  "email": "admin@feuerfest.dev"
}
```

```console
RSASHA256(
  base64UrlEncode(header) + "." +
  base64UrlEncode(payload),

```

```json
{
  "e": "AQAB",
  "kty": "RSA",
  "n": "8Pue9CTRKBf8ZGopqiL8qcjLvw2s0ohBLWCLLxRFdJXUM2am_6ahFDGn7B8QHTMPc_XoaSaUPAeT4wqDJ1S38qC97853BlldXx916y37HWoKMUvqC5ci93sZMSebFlIXEpbr2_5RnmNUe_5FR2zoFnCGnD0hvXEkbkbeHU3C-3ZXlvqsuhkJz8aZMlpnI4jNVZTEtm8Zr10punl2A0i9Wx8qjBJTpJKBOVD25pA3kWW8PMGs_c7NNL9uxo6xUvNlxdmxd3Bj7bem62AUS4THjk88CBjWdO_wBqvbtrx00WPbg_buMDFhqE5SU80O9rJuXjek197Nyv_5-EcWoE9M5Q"
}
```

```console
)
```

When a website request is started with an expired JWT Token.

```console
[11:42:55 INF] Now listening on: http://localhost:8080 <s:Microsoft.Hosting.Lifetime>
[11:42:55 INF] Application started. Press Ctrl+C to shut down. <s:Microsoft.Hosting.Lifetime>
[11:42:55 INF] Hosting environment: Development <s:Microsoft.Hosting.Lifetime>
[11:42:55 INF] Content root path: D:\repos\com.github\lungmuss\lungmuss.frontend.dashboard\Lungmuss.Frontend.Dashboard <s:Microsoft.Hosting.Lifetime>
[11:42:59 INF] Request starting HTTP/1.1 GET http://localhost:8080/ - - <s:Microsoft.AspNetCore.Hosting.Diagnostics>
[11:42:59 INF] Failed to validate the token. <s:Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler>
Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException: IDX10223: Lifetime validation failed. The token is expired. ValidTo (UTC): '25.07.2023 09:46:46', Current time (UTC): '07.08.2023 09:42:59'.
   at Microsoft.IdentityModel.Tokens.Validators.ValidateLifetime(Nullable`1 notBefore, Nullable`1 expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
   at System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.ValidateLifetime(Nullable`1 notBefore, Nullable`1 expires, JwtSecurityToken jwtToken, TokenValidationParameters validationParameters)
   at System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.ValidateTokenPayload(JwtSecurityToken jwtToken, TokenValidationParameters validationParameters, BaseConfiguration configuration)
   at System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.ValidateJWS(String token, TokenValidationParameters validationParameters, BaseConfiguration currentConfiguration, SecurityToken& signatureValidatedToken, ExceptionDispatchInfo& exceptionThrown)
--- End of stack trace from previous location ---
   at System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.ValidateToken(String token, JwtSecurityToken outerToken, TokenValidationParameters validationParameters, SecurityToken& signatureValidatedToken)
   at System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.ValidateToken(String token, TokenValidationParameters validationParameters, SecurityToken& validatedToken)
   at Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler.HandleAuthenticateAsync()
[11:42:59 INF] Bearer was not authenticated. Failure message: IDX10223: Lifetime validation failed. The token is expired. ValidTo (UTC): '25.07.2023 09:46:46', Current time (UTC): '07.08.2023 09:42:59'. <s:Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler>
[11:42:59 INF] Authorization failed. These requirements were not met:
DenyAnonymousAuthorizationRequirement: Requires an authenticated user. <s:Microsoft.AspNetCore.Authorization.DefaultAuthorizationService>
[11:42:59 INF] AuthenticationScheme: Bearer was challenged. <s:Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler>
[11:42:59 INF] Request finished HTTP/1.1 GET http://localhost:8080/ - - - 401 0 - 672.1576ms <s:Microsoft.AspNetCore.Hosting.Diagnostics>
```
